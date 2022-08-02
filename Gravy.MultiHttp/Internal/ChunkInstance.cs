namespace Gravy.MultiHttp.Internal;

internal class ChunkInstance : IChunkInstance
{
    public int ChunkIndex { get; }
    public long StartByte { get; }
    public long EndByte { get; }
    
    public Status Status { get; internal set; }
    
    public int ElapsedMilliseconds => Progress.ElapsedMilliseconds;
    public long CompletedBytes => Progress.CompletedBytes;
    public double CurrentBytesPerSecond => Progress.CurrentSpeed;
    public double AverageBytesPerSecond => Progress.AverageSpeed;
    public double OverallBytesPerSecond => Progress.OverallSpeed;
    
    public long TotalBytes => EndByte - StartByte + 1;

    internal ChunkProgress Progress;
    
    public ChunkInstance(int chunkIndex, long startByte, long endByte)
    {
        if (startByte < 0)
            throw new ArgumentOutOfRangeException(nameof(startByte));
        if (endByte < startByte)
            throw new ArgumentOutOfRangeException(nameof(endByte));
        Status = Status.Waiting;
        ChunkIndex = chunkIndex;
        StartByte = startByte;
        EndByte = endByte;
        Progress = new();
    }
}