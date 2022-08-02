namespace Gravy.MultiHttp.Interfaces;

public interface IChunkInstance : ITrackable
{
    Status Status { get; }
    long StartByte { get; }
    long EndByte { get; }
}