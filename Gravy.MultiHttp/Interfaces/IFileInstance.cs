using JetBrains.Annotations;

namespace Gravy.MultiHttp.Interfaces;

[PublicAPI]
public interface IFileInstance : ITrackable, ITaskable
{
    Guid Id { get; }
    DownloaderStatus Status { get; }
    IDownloadDefinition Definition { get; }
    IReadOnlyList<IChunkInstance> Chunks { get; }
}