namespace Gravy.MultiHttp.Interfaces;

public interface IFileInstance : ITrackable, IWaitable
{
    Guid Id { get; }
    Status Status { get; }
    IDownloadDefinition Definition { get; }
    IReadOnlyList<IChunkInstance> Chunks { get; }
}