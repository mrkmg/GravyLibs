namespace Gravy.MultiHttp.Internal;

internal struct ProgressTracker
{
    public long CompletedBytes => _completedBytes;
    public int ElapsedMilliseconds => _elapsedMilliseconds;
    public double CurrentSpeed { get; private set; }
    public double AverageSpeed { get; private set; }
    public double OverallSpeed { get; private set; }

    private int _startTick;
    private int _elapsedMilliseconds;
    private int _currentTicks;
    private long _currentBytes;
    private long _completedBytes;

    public ProgressTracker()
    {
        _startTick = 0;
        _currentTicks = 0;
        _currentBytes = 0;
        _completedBytes = 0;
        _elapsedMilliseconds = 0;
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
        _elapsedMilliseconds = Environment.TickCount - _startTick + 1;
        
        OverallSpeed = (double)Interlocked.Add(ref _completedBytes, bytesWritten) / _elapsedMilliseconds;
        
        Interlocked.Add(ref _currentBytes, bytesWritten);
        double timeInMs = Environment.TickCount - _currentTicks + 1;
        if (timeInMs < 1000) return false;
        Interlocked.Exchange(ref _currentTicks, Environment.TickCount);
        CurrentSpeed = Interlocked.Exchange(ref _currentBytes, 0) * 1000 / timeInMs; 
        AverageSpeed = AverageSpeed == 0 ? CurrentSpeed : AverageSpeed * 0.95 + CurrentSpeed * 0.05;
        return true;
    }
    
    public void Finished()
    {
        _elapsedMilliseconds = Environment.TickCount - _startTick + 1;
        OverallSpeed = (double)_completedBytes / _elapsedMilliseconds;
    }
}