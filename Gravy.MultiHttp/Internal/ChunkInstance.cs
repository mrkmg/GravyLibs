namespace Gravy.MultiHttp.Internal;

internal class ChunkInstance : IChunkInstance
{
    public int ChunkIndex { get; }
    public long StartByte { get; }
    public long EndByte { get; }
    
    public DownloaderStatus Status { get; private set; }
    
    public int ElapsedMilliseconds => _progress.ElapsedMilliseconds;
    public long CompletedBytes => _progress.CompletedBytes;
    public double CurrentBytesPerSecond => _progress.CurrentSpeed;
    public double AverageBytesPerSecond => _progress.AverageSpeed;
    public double OverallBytesPerSecond => _progress.OverallSpeed;
    
    public long TotalBytes => EndByte - StartByte + 1;
    public Task CompletionTask => _completionSource.Task;

    private readonly TaskCompletionSource _completionSource = new();
    private ChunkProgress _progress;
    
    
    public ChunkInstance(int chunkIndex, long startByte, long endByte)
    {
        if (startByte < 0)
            throw new ArgumentOutOfRangeException(nameof(startByte));
        if (endByte < startByte)
            throw new ArgumentOutOfRangeException(nameof(endByte));
        Status = DownloaderStatus.Waiting;
        ChunkIndex = chunkIndex;
        StartByte = startByte;
        EndByte = endByte;
        _progress = new();
    }

    internal void Started()
    {
        Status = DownloaderStatus.InProgress;
        _progress.Started();
    }

    internal void ApplyProgress(long bytes)
    {
        _progress.ApplyProgress(bytes);
    }

    internal void Finished()
    {
        Status = DownloaderStatus.Complete;
        _progress.Finished();
        _completionSource.SetResult();
    }

    internal void Errored(Exception e)
    {
        Status = DownloaderStatus.Errored;
        _completionSource.SetException(e);
    }
}