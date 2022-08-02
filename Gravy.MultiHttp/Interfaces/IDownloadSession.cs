namespace Gravy.MultiHttp.Interfaces;

public interface IDownloadSession : IDisposable, IWaitable
{
    event EventHandler? OnStarted;
    event EventHandler<IOverallProgress>? OnProgress;
    event EventHandler<bool>? OnEnded;
    event EventHandler<Exception>? OnError;
    
    event EventHandler<IFileInstance>? OnFileStarted;
    event EventHandler<IFileProgress>? OnFileProgress;
    event EventHandler<IFileInstance>? OnFileEnded;
    event EventHandler<(IFileInstance File, Exception Exception)>? OnFileError;
    
    bool IsRunning { get; }
    IFileInstance AddDownload(IDownloadDefinition definition);
    IFileInstance AddDownload(IDownloadDefinition definition, CancellationToken token);
    IEnumerable<IFileInstance> AddDownloads(IEnumerable<IDownloadDefinition> definitions);
    IEnumerable<IFileInstance> AddDownloads(IEnumerable<IDownloadDefinition> definitions, CancellationToken token);
    void Start();
    void Stop();
    Task StartAsync();
    Task StartAsync(CancellationToken token);
}