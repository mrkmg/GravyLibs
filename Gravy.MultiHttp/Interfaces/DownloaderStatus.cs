using JetBrains.Annotations;

namespace Gravy.MultiHttp.Interfaces;

[PublicAPI]
public enum DownloaderStatus
{
    Errored = -1,
    Waiting = 0,
    InProgress = 1,
    Complete = 2,
}