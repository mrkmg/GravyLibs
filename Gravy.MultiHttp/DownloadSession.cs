using Gravy.MultiHttp.Internal;
using JetBrains.Annotations;

namespace Gravy.MultiHttp;

[PublicAPI]
public sealed class DownloadSession : IDownloadSession
{
    public DownloadSession(long maxChunkSize,
                             int maxConcurrency,
                             FileWriterType fileWriterType,
                             HttpClient? client = null)
    {
        MaxChunkSize = maxChunkSize;
        MaxConcurrency = maxConcurrency;
        _fileWriterType = fileWriterType;
        _client = client ?? new();
        _urlProcessor = new(_client);
        _overallProgress = new();
        _downloadAggregator = new(Enumerable.Repeat<Action>(RunnerThread, MaxConcurrency).ToArray());
    }
    
    
    private const int DefaultBufferSize = 2048; // 2KB
    private const int MaxBufferSize = 8 * 1024 * 1024; // 8MB
    private const int MinBufferSize = 1024; // 1KB

    private readonly FileWriterType _fileWriterType;
    private readonly HttpClient _client;
    private readonly FileDefinitions _fileDefinitions = new();
    private readonly ActionPool _downloadAggregator;
    private readonly UrlProcessor _urlProcessor;

    private CancellationTokenSource _runningCancellationTokenSource = new();
    private OverallProgress _overallProgress;

    public event EventHandler? OnStarted;
    public event EventHandler<IOverallProgress>? OnProgress;
    public event EventHandler<bool>? OnEnded;
    public event EventHandler<Exception>? OnError;
    
    public event EventHandler<IFileInstance>? OnFileStarted;
    public event EventHandler<IFileProgress>? OnFileProgress;
    public event EventHandler<IFileInstance>? OnFileEnded;
    public event EventHandler<(IFileInstance File, Exception Exception)>? OnFileError;
    
    public bool IsRunning { get; private set; }
    public long MaxChunkSize { get; }
    public int MaxConcurrency { get; }
    public Task CompletionTask => _downloadAggregator.Task;

    public IFileInstance AddDownload(IDownloadDefinition definition, CancellationToken? token = null)
    {
        if (!definition.Overwrite && File.Exists(definition.DestinationFilePath))
            throw new ArgumentException($"{definition.DestinationFilePath} already exists and overwrite is false");

        var file = _urlProcessor.ProcessDownload(definition, MaxChunkSize, _fileWriterType, token);
        
        _overallProgress.FileAdded(file.TotalBytes);
        _fileDefinitions.AddInstance(file);
        return file;
    }

    public IEnumerable<IFileInstance> AddDownloads(IEnumerable<IDownloadDefinition> definitions, CancellationToken? token = null)
        => definitions
            .AsParallel()
            .WithDegreeOfParallelism(MaxConcurrency)
            .Select(x => AddDownload(x, token))
            .ToList();

    public void Start()
    {
        if (IsRunning) return;
        IsRunning = true;
        _runningCancellationTokenSource = new();
        _overallProgress.Started();
        OnStarted?.Invoke(this, EventArgs.Empty);
        _downloadAggregator.Start();
    }

    public void Stop()
    {
        if (!IsRunning) return;
        _runningCancellationTokenSource.Cancel();
        IsRunning = false;
    }

    public Task StartAsync()
        => StartAsync(CancellationToken.None);

    public async Task StartAsync(CancellationToken stopToken)
    {
        if (IsRunning) return;
        Start();
        try
        {
            await CompletionTask.WaitAsync(stopToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Stop();
            throw;
        }
    }

    public void Dispose()
    {
        _client.Dispose();
        _fileDefinitions.Dispose();
        _runningCancellationTokenSource.Dispose();
    }
    
    private void ChunkStarted(FileInstance file, ChunkInstance chunk)
    {
        if (file.CheckAndSetStarted())
        {
            _overallProgress.FileStarted(file);
            OnFileStarted?.Invoke(this, file);
        }

        file.Progress.ThreadStarted();
        file.Writer.StartChunk(chunk.ChunkIndex);
        chunk.Started();
    }

    private void ChunkProgressed(FileInstance file, ChunkInstance chunk, long bytesMoved)
    {
        chunk.ApplyProgress(bytesMoved);
        if (file.Progress.ApplyProgress(bytesMoved)) OnFileProgress?.Invoke(this, file.Progress);
        if (_overallProgress.ApplyProgress(bytesMoved)) OnProgress?.Invoke(this, _overallProgress);
    }
    
    private void ChunkCompleted(FileInstance file, ChunkInstance chunk)
    {
        file.Writer.FinalizeChunk(chunk.ChunkIndex);
        chunk.Finished();
        if (!file.CheckAndSetCompleted()) return;
        
        _overallProgress.FileCompleted(file.Id);
        file.Progress.ChunkCompleted();
        file.Progress.ThreadFinished();
        
        OnFileEnded?.Invoke(this, file);
        if (!_fileDefinitions.AllFilesCompleted()) return;
        
        _overallProgress.Finished();
        OnEnded?.Invoke(this, true);
    }

    private void ChunkErrored(FileInstance file, ChunkInstance chunk, Exception error)
    {
        if (chunk.Status != DownloaderStatus.Complete)
            chunk.Errored(error);
        if (file.Status != DownloaderStatus.Complete)
            file.SetErrored(error);
        OnError?.Invoke(this, error);
        OnEnded?.Invoke(this, false);
        OnFileError?.Invoke(this, (file, error));
        OnFileEnded?.Invoke(this, file);
        if (!_runningCancellationTokenSource.IsCancellationRequested)
            _runningCancellationTokenSource.Cancel();
    }

    private void RunnerThread()
    {
        while (!_runningCancellationTokenSource.IsCancellationRequested)
        {
            if (!_fileDefinitions.TryGetWaitingChunkToProcess(out var file, out var chunk))
                return;

            try
            {
                RunChunkDownload(file, chunk);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                ChunkErrored(file, chunk, e);
                throw;
            }
        }
    }

    private void RunChunkDownload(FileInstance file, ChunkInstance chunk)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, file.Definition.SourceUri);
        if (file.ChunksInternal.Length > 1)
            request.Headers.Range = new(chunk.StartByte, chunk.EndByte);
        using var response = _client.Send(request, HttpCompletionOption.ResponseHeadersRead, _runningCancellationTokenSource.Token);
        if (!response.IsSuccessStatusCode)
            throw new("TODO - Non-SuccessfulStatusCode");
        
        ChunkStarted(file, chunk);
        _overallProgress.ThreadStarted();

        var readStream = response.Content.ReadAsStream(_runningCancellationTokenSource.Token);

        var buffer = new DynamicallySizedBuffer(DefaultBufferSize, 10, 500, MinBufferSize, MaxBufferSize);
        int readBytes;
        while ((readBytes = readStream.Read(buffer.Span)) > 0)
        {
            if (_runningCancellationTokenSource.IsCancellationRequested)
                throw new OperationCanceledException();
            
            file.Writer.WriteToChunk(chunk.ChunkIndex, buffer.Memory[..readBytes]);
            ChunkProgressed(file, chunk, readBytes);
            buffer.TriggerTick();
        }

        _overallProgress.ThreadFinished();
        ChunkCompleted(file, chunk);
    }
}

public class MissingContentLengthException : Exception
{
    public MissingContentLengthException(string sourceUri)
    {
        SourceUri = sourceUri;
    }
    
    public string SourceUri { get; }
    public override string Message => $"Missing Content-Length header in response for {SourceUri}";
}