namespace Gravy.MultiHttp;

public class DownloadDefinition : IDownloadDefinition
{
    public string SourceUri { get; init; } = string.Empty;
    public string DestinationFilePath { get; init; } = string.Empty;
    public bool Overwrite { get; init; } = true;
}