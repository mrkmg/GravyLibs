namespace Gravy.MultiHttp;

public class DownloadDefinition : IDownloadDefinition
{
    public string SourceUri { get; init; }
    public string DestinationFilePath { get; init; }
    public bool Overwrite { get; init; } = true;
}