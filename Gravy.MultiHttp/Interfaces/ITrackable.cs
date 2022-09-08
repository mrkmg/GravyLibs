using JetBrains.Annotations;

namespace Gravy.MultiHttp.Interfaces;

[PublicAPI]
public interface ITrackable
{
    long TotalBytes { get; }
    long CompletedBytes { get; }
    long RemainingBytes => TotalBytes - CompletedBytes;
    int ElapsedMilliseconds { get; }
    double CurrentBytesPerSecond { get; }
    double AverageBytesPerSecond { get; }
    double OverallBytesPerSecond { get; }
}

[PublicAPI]
public static class TrackableExtensions
{
    public static double RatioComplete(this ITrackable trackable) 
        => trackable.CompletedBytes / (double)trackable.TotalBytes;
    
    public static TimeSpan EstimatedTimeRemaining(this ITrackable trackable, 
                                                  EstimationSource estimationSource = EstimationSource.Average) 
        => TimeSpan.FromSeconds(estimationSource switch
        {
            EstimationSource.Current => trackable.CurrentBytesPerSecond,
            EstimationSource.Average => trackable.AverageBytesPerSecond,
            EstimationSource.Overall => trackable.OverallBytesPerSecond,
            _ => throw new ArgumentOutOfRangeException(nameof(estimationSource), estimationSource, null),
        } / trackable.RemainingBytes);
}

[PublicAPI]
public enum EstimationSource
{
    Current,
    Average,
    Overall,
}