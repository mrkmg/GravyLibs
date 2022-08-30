using Gravy.MultiHttp.Internal.Writers;

namespace Gravy.MultiHttp.Internal;

internal class FileInstance : IFileInstance, IDisposable
{
    public Guid Id { get; }
    public IDownloadDefinition Definition { get; }
    public long TotalBytes { get; }
    public Status Status { get; private set; }
    
    public long CompletedBytes => Progress.CompletedBytes;
    public double CurrentBytesPerSecond => Progress.CurrentBytesPerSecond;
    public double AverageBytesPerSecond => Progress.AverageBytesPerSecond;
    public double OverallBytesPerSecond => Progress.OverallBytesPerSecond;
    public IReadOnlyList<IChunkInstance> Chunks => ChunksInternal;
    public int ElapsedMilliseconds => Progress.ElapsedMilliseconds;

    private readonly object _lockObject = new();
    private readonly TaskCompletionSource _completionSource = new();
    
    internal readonly ChunkInstance[] ChunksInternal;
    internal IFileWriter Writer { get; }
    internal FileProgress Progress;

    public FileInstance(Guid id, IDownloadDefinition definition, FileWriterType writerType, long totalBytes, ChunkInstance[] chunks)
    {
        Id = id;
        Status = Status.Waiting;
        Definition = definition;
        TotalBytes = totalBytes;
        ChunksInternal = chunks;
        Progress = new(this, totalBytes, chunks.Length);
        Writer = writerType switch
        {
            FileWriterType.TemporaryFiles => new TemporaryFilesWriter(this),
            FileWriterType.Hybrid => new HybridFileWriter(this),
            FileWriterType.InMemory => new MemoryFileWriter(this),
            FileWriterType.Direct => new DirectWriter(this),
            FileWriterType.Sequential => new SequentialWriter(this),
            _ => throw new ArgumentOutOfRangeException(nameof(writerType), writerType, null)
        };
    }

    public async Task WaitAsync()
        => await _completionSource.Task.ConfigureAwait(false);

    public async Task WaitAsync(CancellationToken token)
        => await _completionSource.Task.WaitAsync(token).ConfigureAwait(false);

    internal void SetErrored(Exception error)
    {
        Status = Status.Errored;
        if (error is OperationCanceledException)
            _completionSource.TrySetCanceled();
        else
            _completionSource.TrySetException(error);
    }

    internal bool CheckAndSetStarted()
    {
        lock (_lockObject)
        {
            if (Status != Status.Waiting)
                return false;
            Status = Status.InProgress;
            Writer.StartFile();
            Progress.Started();
            return true;
        }
    }
    
    internal bool CheckAndSetCompleted()
    {
        lock (_lockObject)
        {
            if (Status == Status.Complete || ChunksInternal.Any(x => x.Status != Status.Complete))
                return false;
            Status = Status.Complete;
            _completionSource.SetResult();
            Progress.Finished();
            Writer.FinalizeFile();
            return true;
        }
    }

    public void Dispose()
        => Writer.Dispose();

}