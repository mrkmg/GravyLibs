namespace Gravy.MultiHttp.Interfaces;

public interface IDownloadDefinition
{
    string SourceUri { get; }
    string DestinationFilePath { get; }
    bool Overwrite { get; }
}