namespace Gravy.MultiHttp;

public interface IDownloadBuilder
{
    IDownloadBuilder WithMaxChunkSize(long maxChunkSize);
    IDownloadBuilder WithMaxConcurrency(int maxConcurrency);
    IDownloadBuilder WithFileDestinationType(FileWriterType fileWriterType);
    IDownloadBuilder ConfigureHttpClient(Action<HttpClient> configure);
    IDownloadBuilder OnStarted(Action handler);
    IDownloadBuilder OnProgress(Action<IOverallProgress> handler);
    IDownloadBuilder OnEnded(Action<bool> handler);
    IDownloadBuilder OnError(Action<Exception> handler);
    IDownloadBuilder OnFileStarted(Action<IFileInstance> handler);
    IDownloadBuilder OnFileProgress(Action<IFileProgress> handler);
    IDownloadBuilder OnFileEnded(Action<IFileInstance> handler);
    IDownloadBuilder OnFileError(Action<(IFileInstance File, Exception Exception)> handler);
    IDownloadBuilder AddDownload(IDownloadDefinition download);
    IDownloadBuilder AddDownloads(IEnumerable<IDownloadDefinition> downloads);
    IPathedDownloadBuilder WithDestinationPath(string destinationPath);
    IDownloadSession Build();
}

public interface IPathedDownloadBuilder : IDownloadBuilder
{
    IPathedDownloadBuilder AddDownload(string url, bool overwrite = false);
    IPathedDownloadBuilder AddDownloads(IEnumerable<string> urls, bool overwrite = false);
}