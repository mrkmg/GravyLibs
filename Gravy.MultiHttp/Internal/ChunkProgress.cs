namespace Gravy.MultiHttp.Internal;

internal struct ChunkProgress
{
    public long CompletedBytes => _progressTracker.CompletedBytes;
    public double CurrentSpeed => _progressTracker.CurrentSpeed;
    public double AverageSpeed => _progressTracker.AverageSpeed;
    public double OverallSpeed => _progressTracker.OverallSpeed;
    public int ElapsedMilliseconds => _progressTracker.ElapsedMilliseconds;

    private ProgressTracker _progressTracker;
    
    public ChunkProgress()
    {
        _progressTracker = new();
    }

    public void Started()
        => _progressTracker.Started();

    public bool ApplyProgress(long bytesWritten)
        => _progressTracker.ApplyProgress(bytesWritten);
    
    public void Finished()
        => _progressTracker.Finished();
}