namespace Gravy.MultiHttp.Internal;

internal struct ProgressTracker
{
    private static readonly int AverageIntervalMs = 5000;
    private static readonly int MinimumReportingIntervalMs = 500;
    
    public long CompletedBytes => _completedBytes;
    public int ElapsedMilliseconds { get; private set; }
    public double CurrentSpeed { get; private set; }
    public double AverageSpeed { get; private set; }
    public double OverallSpeed { get; private set; }

    private int _startTick;
    private int _currentTicks;
    private long _currentBytes;
    private long _completedBytes;

    public ProgressTracker()
    {
        _startTick = 0;
        _currentTicks = 0;
        _currentBytes = 0;
        _completedBytes = 0;
        ElapsedMilliseconds = 0;
        AverageSpeed = 0;
        CurrentSpeed = 0;
        OverallSpeed = 0;
    }
    
    public void Started()
    {
        _currentBytes = 0;
        _currentTicks = Environment.TickCount;
        _startTick = Environment.TickCount;
    }
    
    public bool ApplyProgress(long bytesWritten)
    {
        ElapsedMilliseconds = Environment.TickCount - _startTick + 1;
        
        OverallSpeed = (double)Interlocked.Add(ref _completedBytes, bytesWritten) / ElapsedMilliseconds;
        
        Interlocked.Add(ref _currentBytes, bytesWritten);
        double timeInMs = Environment.TickCount - _currentTicks + 1;
        if (timeInMs < MinimumReportingIntervalMs) return false;
        Interlocked.Exchange(ref _currentTicks, Environment.TickCount);
        CurrentSpeed = Interlocked.Exchange(ref _currentBytes, 0) * 1000 / timeInMs;
        if (AverageSpeed == 0) AverageSpeed = CurrentSpeed;
        else
        {
            var ratio = Math.Min(1.0, timeInMs/AverageIntervalMs);
            AverageSpeed = (AverageSpeed * (1 - ratio) + CurrentSpeed * ratio);
        }
        return true;
    }
    
    public void Finished()
    {
        ElapsedMilliseconds = Environment.TickCount - _startTick + 1;
        OverallSpeed = (double)_completedBytes / ElapsedMilliseconds;
    }
}