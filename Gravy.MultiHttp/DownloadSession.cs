using System.Diagnostics;
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
        _overallProgress = new();
        _downloadAggregator = new(Enumerable.Repeat<Action>(RunnerThread, MaxConcurrency).ToArray());
    }
    
    private const string HeaderAcceptRangesKey = "Accept-Ranges";
    private const int DefaultBufferSize = 2048;
    private const int MaxBufferSize = 1024 * 1024;
    private const int MinBufferSize = 1024;

    private readonly FileWriterType _fileWriterType;
    private readonly HttpClient _client;
    private readonly FileDefinitions _fileDefinitions = new();
    private readonly ActionPool _downloadAggregator;

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

    public IFileInstance AddDownload(IDownloadDefinition definition)
        => AddDownload(definition, CancellationToken.None);

    public IFileInstance AddDownload(IDownloadDefinition definition, CancellationToken token)
    {
        if (!definition.Overwrite && File.Exists(definition.DestinationFilePath))
            throw new ArgumentException($"{definition.DestinationFilePath} already exists and overwrite is false");

        var (totalBytes, acceptsRange) = GetHeaderData(definition, token);
        var numChunksNeeded = acceptsRange ? (int)Math.Ceiling(totalBytes / (double)MaxChunkSize) : 1;
        var chunks = MakeChunks(totalBytes, numChunksNeeded);
        var file = new FileInstance(Guid.NewGuid(), definition, _fileWriterType, totalBytes, chunks);

        Interlocked.Increment(ref _overallProgress.TotalFilesInternal);
        Interlocked.Add(ref _overallProgress.TotalBytesInternal, file.TotalBytes);
        _fileDefinitions.AddInstance(file);
        return file;
    }

    private static ChunkInstance[] MakeChunks(long totalBytes, long numChunksNeeded)
    {
        var chunkSize = totalBytes / numChunksNeeded;
        var chunks = new ChunkInstance[numChunksNeeded];
        var currentChunkIndex = 0;
        while (currentChunkIndex < chunks.Length - 1)
        {
            chunks[currentChunkIndex] = new(currentChunkIndex, currentChunkIndex * chunkSize, (currentChunkIndex + 1) * chunkSize - 1);
            currentChunkIndex++;
        }
        chunks[currentChunkIndex] = new(currentChunkIndex, currentChunkIndex * chunkSize, totalBytes - 1);
        return chunks;
    }

    private (long totalBytes, bool acceptsRange) GetHeaderData(IDownloadDefinition definition, CancellationToken token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Head, definition.SourceUri);
        using var responseMessage = _client.Send(requestMessage, HttpCompletionOption.ResponseHeadersRead, token);

        if (!responseMessage.IsSuccessStatusCode)
            throw new HttpRequestException($"HEAD failed with code \"{responseMessage.StatusCode}\" for \"{definition.SourceUri}\"", null, responseMessage.StatusCode);

        if (responseMessage.Content.Headers.ContentLength is null)
            throw new MissingContentLengthException(definition.SourceUri);

        var totalBytes = responseMessage.Content.Headers.ContentLength.Value;
        if (totalBytes <= 0)
            throw new MissingContentLengthException(definition.SourceUri);

        var acceptsRange = responseMessage.Headers.Contains(HeaderAcceptRangesKey) &&
                           responseMessage.Headers.GetValues(HeaderAcceptRangesKey).All(x => x != "bytes");
        return (totalBytes, acceptsRange);
    }

    public IEnumerable<IFileInstance> AddDownloads(IEnumerable<IDownloadDefinition> definitions)
        => AddDownloads(definitions, CancellationToken.None);

    public IEnumerable<IFileInstance> AddDownloads(IEnumerable<IDownloadDefinition> definitions, CancellationToken token)
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
            await WaitAsync(stopToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Stop();
            throw;
        }
    }

    public Task WaitAsync()
        => WaitAsync(CancellationToken.None);

    public async Task WaitAsync(CancellationToken token)
    {
        await _downloadAggregator.Task.WaitAsync(token).ConfigureAwait(false);
    }

    public void Dispose()
    {
        _client.Dispose();
        _fileDefinitions.Dispose();
        _runningCancellationTokenSource.Dispose();
    }
    
    private void ChunkStarted(FileInstance file, ChunkInstance chunk)
    {
        if (file.CheckStarted())
        {
            _overallProgress.ActiveFilesInternal.TryAdd(file.Id, file);
            OnFileStarted?.Invoke(this, file);
        }
        
        Interlocked.Increment(ref file.Progress.ActiveThreadsInternal);
        file.Writer.StartChunk(chunk.ChunkIndex);
        chunk.Progress.Started();
    }

    private void ChunkProgressed(FileInstance file, ChunkInstance chunk, long bytesMoved)
    {
        chunk.Progress.ApplyProgress(bytesMoved);
        if (file.Progress.ApplyProgress(bytesMoved)) OnFileProgress?.Invoke(this, file.Progress);
        if (_overallProgress.ApplyProgress(bytesMoved)) OnProgress?.Invoke(this, _overallProgress);
    }
    
    private void ChunkCompleted(FileInstance file, ChunkInstance chunk)
    {
        chunk.Status = Status.Complete;
        chunk.Progress.Finished();
        file.Writer.FinalizeChunk(chunk.ChunkIndex);
        if (!file.CheckCompleted())
            return;
        _overallProgress.ActiveFilesInternal.TryRemove(file.Id, out _);
        Interlocked.Increment(ref _overallProgress.CompletedFilesInternal);
        Interlocked.Increment(ref file.Progress.CompletedChunksInternal);
        Interlocked.Decrement(ref file.Progress.ActiveThreadsInternal);
        
        OnFileEnded?.Invoke(this, file);
        if (!_fileDefinitions.AllFilesCompleted()) return;
        
        _overallProgress.Finished();
        OnEnded?.Invoke(this, true);
    }

    private void ChunkErrored(FileInstance file, ChunkInstance chunk, Exception error)
    {
        chunk.Status = Status.Errored;
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
                chunk.Status = Status.Errored;
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
        Interlocked.Increment(ref _overallProgress.ActiveThreadsInternal);

        var readStream = response.Content.ReadAsStream(_runningCancellationTokenSource.Token);

        var buffer = new DynamicallySizedBuffer(DefaultBufferSize, 10, 500, MinBufferSize, MaxBufferSize);
        int readBytes;
        while ((readBytes = readStream.Read(buffer.Memory.Span)) > 0)
        {
            if (_runningCancellationTokenSource.IsCancellationRequested)
                throw new OperationCanceledException();
            
            file.Writer.WriteChunk(chunk.ChunkIndex, buffer.Memory[..readBytes]);
            ChunkProgressed(file, chunk, readBytes);
            buffer.TriggerTick();
        }

        Interlocked.Decrement(ref _overallProgress.ActiveThreadsInternal);
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