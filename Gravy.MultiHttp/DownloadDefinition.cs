namespace Gravy.MultiHttp;

public class DownloadDefinition : IDownloadDefinition
{
    public required string SourceUri { get; init; }
    public required string DestinationFilePath { get; init; }
    public bool Overwrite { get; init; } = true;
}