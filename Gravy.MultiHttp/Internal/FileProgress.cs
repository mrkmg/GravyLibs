namespace Gravy.MultiHttp.Internal;

internal struct FileProgress : IFileProgress
{
    public IFileInstance Instance { get; }
    public long TotalBytes { get; }
    public int TotalChunks { get; }

    public int CompletedChunks => _completedChunksInternal;
    public int ActiveChunks => _activeThreadsInternal;

    public long CompletedBytes => _progressTracker.CompletedBytes;
    public double CurrentBytesPerSecond => _progressTracker.CurrentSpeed;
    public double AverageBytesPerSecond => _progressTracker.AverageSpeed;
    public double OverallBytesPerSecond => _progressTracker.OverallSpeed;
    public int ElapsedMilliseconds => _progressTracker.ElapsedMilliseconds;

    private ProgressTracker _progressTracker;
    private int _completedChunksInternal = 0;
    private int _activeThreadsInternal = 0;

    public FileProgress(IFileInstance instance, long totalBytes, int totalChunks)
    {
        Instance = instance;
        TotalBytes = totalBytes;
        TotalChunks = totalChunks;
        _progressTracker = new();
    }

    public void Started()
        => _progressTracker.Started();

    public bool ApplyProgress(long bytesWritten)
        => _progressTracker.ApplyProgress(bytesWritten);
    
    public void Finished()
        => _progressTracker.Finished();
    
    public void ChunkCompleted()
        => Interlocked.Increment(ref _completedChunksInternal);

    public void ThreadStarted()
        => Interlocked.Increment(ref _activeThreadsInternal);

    public void ThreadFinished()
        => Interlocked.Decrement(ref _activeThreadsInternal);
}