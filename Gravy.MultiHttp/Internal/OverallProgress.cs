using System.Collections.Concurrent;

namespace Gravy.MultiHttp.Internal;

internal struct OverallProgress : IOverallProgress
{
    public long TotalBytes => _totalBytesInternal;
    public int TotalFiles => _totalFilesInternal;
    public int CompletedFiles => _completedFilesInternal;
    public int ActiveThreads => _activeThreadsInternal;
    public IEnumerable<IFileInstance> ActiveFiles => _activeFilesInternal.Values.ToArray();

    public long CompletedBytes => _progressTracker.CompletedBytes;
    public double CurrentBytesPerSecond => _progressTracker.CurrentSpeed;
    public double AverageBytesPerSecond => _progressTracker.AverageSpeed;
    public double OverallBytesPerSecond => _progressTracker.OverallSpeed;
    public int ElapsedMilliseconds => _progressTracker.ElapsedMilliseconds;
    
    private ProgressTracker _progressTracker;

    private readonly ConcurrentDictionary<Guid, FileInstance> _activeFilesInternal;
    private long _totalBytesInternal;
    private int _totalFilesInternal;
    private int _completedFilesInternal;
    private int _activeThreadsInternal;

    public OverallProgress()
    {
        _totalBytesInternal = 0;
        _totalFilesInternal = 0;
        _completedFilesInternal = 0;
        _activeThreadsInternal = 0;
        _activeFilesInternal = new();
        _progressTracker = new();
    }

    internal void Started()
        => _progressTracker.Started();

    internal bool ApplyProgress(long bytesWritten)
        => _progressTracker.ApplyProgress(bytesWritten);

    internal void Finished()
        => _progressTracker.Finished();

    internal void FileAdded(long fileTotalBytes)
    {
        Interlocked.Increment(ref _totalFilesInternal);
        Interlocked.Add(ref _totalBytesInternal, fileTotalBytes);
    }

    internal void FileCompleted(Guid fileId)
    {
        Interlocked.Increment(ref _completedFilesInternal);
        _activeFilesInternal.TryRemove(fileId, out _);
    }

    internal void FileStarted(FileInstance file)
    {
        _activeFilesInternal.TryAdd(file.Id, file);
    }
    
    internal void ThreadStarted()
    {
        Interlocked.Increment(ref _activeThreadsInternal);
    }
    
    internal void ThreadFinished()
    {
        Interlocked.Decrement(ref _activeThreadsInternal);
    }
}