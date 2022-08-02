using System.Collections.Concurrent;

namespace Gravy.MultiHttp.Internal;

internal struct OverallProgress : IOverallProgress
{
    public long TotalBytes => TotalBytesInternal;
    public int TotalFiles => TotalFilesInternal;
    public int CompletedFiles => CompletedFilesInternal;
    public int ActiveThreads => ActiveThreadsInternal;
    public IEnumerable<IFileInstance> ActiveFiles => ActiveFilesInternal.Values.ToArray();

    public long CompletedBytes => _progressTracker.CompletedBytes;
    public double CurrentBytesPerSecond => _progressTracker.CurrentSpeed;
    public double AverageBytesPerSecond => _progressTracker.AverageSpeed;
    public double OverallBytesPerSecond => _progressTracker.OverallSpeed;
    public int ElapsedMilliseconds => _progressTracker.ElapsedMilliseconds;
    
    private ProgressTracker _progressTracker;
    
    internal ConcurrentDictionary<Guid, FileInstance> ActiveFilesInternal { get; }
    internal long TotalBytesInternal;
    internal int TotalFilesInternal;
    internal int CompletedFilesInternal;
    internal int ActiveThreadsInternal;

    public OverallProgress()
    {
        TotalBytesInternal = 0;
        TotalFilesInternal = 0;
        CompletedFilesInternal = 0;
        ActiveThreadsInternal = 0;
        ActiveFilesInternal = new();
        _progressTracker = new();
    }

    public void Started()
        => _progressTracker.Started();

    public bool ApplyProgress(long bytesWritten)
        => _progressTracker.ApplyProgress(bytesWritten);

    public void Finished()
        => _progressTracker.Finished();

}