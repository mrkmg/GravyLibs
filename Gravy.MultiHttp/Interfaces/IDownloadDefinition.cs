using JetBrains.Annotations;

namespace Gravy.MultiHttp.Interfaces;

[PublicAPI]
public interface IDownloadDefinition
{
    string SourceUri { get; }
    string DestinationFilePath { get; }
    bool Overwrite { get; }
}