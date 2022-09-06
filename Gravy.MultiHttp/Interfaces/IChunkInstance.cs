using JetBrains.Annotations;

namespace Gravy.MultiHttp.Interfaces;

[PublicAPI]
public interface IChunkInstance : ITrackable, ITaskable
{
    DownloaderStatus Status { get; }
    long StartByte { get; }
    long EndByte { get; }
}