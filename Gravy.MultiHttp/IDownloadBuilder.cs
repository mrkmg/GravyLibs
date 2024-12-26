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
    IPathedDownloadBuilder AddDownload(string url);
    IPathedDownloadBuilder AddDownload(string url, bool overwrite);
    IPathedDownloadBuilder AddDownloads(IEnumerable<string> urls);
    IPathedDownloadBuilder AddDownloads(IEnumerable<string> urls, bool overwrite);
    new IPathedDownloadBuilder WithMaxChunkSize(long maxChunkSize);
    new IPathedDownloadBuilder WithMaxConcurrency(int maxConcurrency);
    new IPathedDownloadBuilder WithFileDestinationType(FileWriterType fileWriterType);
    new IPathedDownloadBuilder ConfigureHttpClient(Action<HttpClient> configure);
    new IPathedDownloadBuilder OnStarted(Action handler);
    new IPathedDownloadBuilder OnProgress(Action<IOverallProgress> handler);
    new IPathedDownloadBuilder OnEnded(Action<bool> handler);
    new IPathedDownloadBuilder OnError(Action<Exception> handler);
    new IPathedDownloadBuilder OnFileStarted(Action<IFileInstance> handler);
    new IPathedDownloadBuilder OnFileProgress(Action<IFileProgress> handler);
    new IPathedDownloadBuilder OnFileEnded(Action<IFileInstance> handler);
    new IPathedDownloadBuilder OnFileError(Action<(IFileInstance File, Exception Exception)> handler);
    new IPathedDownloadBuilder AddDownload(IDownloadDefinition download);
    new IPathedDownloadBuilder AddDownloads(IEnumerable<IDownloadDefinition> downloads);
    new IDownloadSession Build();
}