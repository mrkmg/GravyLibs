namespace Gravy.MultiHttp.Interfaces;

public interface ITrackable
{
    long TotalBytes { get; }
    long CompletedBytes { get; }
    int ElapsedMilliseconds { get; }
    double CurrentBytesPerSecond { get; }
    double AverageBytesPerSecond { get; }
    double OverallBytesPerSecond { get; }
}