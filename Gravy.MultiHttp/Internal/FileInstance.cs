using Gravy.MultiHttp.Internal.Writers;

namespace Gravy.MultiHttp.Internal;

internal class FileInstance : IFileInstance, IDisposable
{
    public Guid Id { get; }
    public IDownloadDefinition Definition { get; }
    public long TotalBytes { get; }
    public DownloaderStatus Status { get; private set; }
    
    public long CompletedBytes => Progress.CompletedBytes;
    public double CurrentBytesPerSecond => Progress.CurrentBytesPerSecond;
    public double AverageBytesPerSecond => Progress.AverageBytesPerSecond;
    public double OverallBytesPerSecond => Progress.OverallBytesPerSecond;
    public IReadOnlyList<IChunkInstance> Chunks => ChunksInternal;
    public int ElapsedMilliseconds => Progress.ElapsedMilliseconds;
    public Task CompletionTask => _completionSource.Task;

    private readonly object _lockObject = new();
    private readonly TaskCompletionSource _completionSource = new();
    
    internal readonly ChunkInstance[] ChunksInternal;
    internal IFileWriter Writer { get; }
    internal FileProgress Progress;

    public FileInstance(Guid id, IDownloadDefinition definition, FileWriterType writerType, long totalBytes, ChunkInstance[] chunks)
    {
        Id = id;
        Status = DownloaderStatus.Waiting;
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
            FileWriterType.DirectSequential => new SequentialWriter(this),
            _ => throw new ArgumentOutOfRangeException(nameof(writerType), writerType, null),
        };
    }

    internal void SetErrored(Exception error)
    {
        Status = DownloaderStatus.Errored;
        if (error is OperationCanceledException)
            _completionSource.TrySetCanceled();
        else
            _completionSource.TrySetException(error);
    }

    internal bool CheckAndSetStarted()
    {
        lock (_lockObject)
        {
            if (Status != DownloaderStatus.Waiting)
                return false;
            Status = DownloaderStatus.InProgress;
            Writer.StartFile();
            Progress.Started();
            return true;
        }
    }
    
    internal bool CheckAndSetCompleted()
    {
        lock (_lockObject)
        {
            if (Status == DownloaderStatus.Complete || ChunksInternal.Any(x => x.Status != DownloaderStatus.Complete))
                return false;
            Status = DownloaderStatus.Complete;
            _completionSource.SetResult();
            Progress.Finished();
            Writer.FinalizeFile();
            return true;
        }
    }

    public void Dispose()
        => Writer.Dispose();

}