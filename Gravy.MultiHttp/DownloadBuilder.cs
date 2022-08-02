namespace Gravy.MultiHttp;

public class DownloadBuilder
{
    private const int DefaultChunkSize = 1024 * 1024 * 20; // 20MB
    private const int DefaultConcurrency = 4;
    
    private long _maxChunkSize = DefaultChunkSize;
    private int _maxConcurrency = DefaultConcurrency;
    private FileWriterType _fileWriterType = FileWriterType.Hybrid;
    private readonly HttpClient _client = new();
    
    private readonly List<Action> _onStartedHandlers = new();
    private readonly List<Action<IOverallProgress>> _onProgressHandler = new();
    private readonly List<Action<bool>> _onEndedHandler = new();
    private readonly List<Action<Exception>> _onErrorHandler = new();
    
    private readonly List<Action<IFileInstance>> _onFileStartedHandler = new();
    private readonly List<Action<IFileProgress>> _onFileProgressHandler = new();
    private readonly List<Action<IFileInstance>> _onFileEndedHandler = new();
    private readonly List<Action<(IFileInstance File, Exception Exception)>> _onFileErrorHandler = new();
    
    private readonly List<DownloadDefinition> _downloads = new();

    public DownloadBuilder WithMaxChunkSize(long maxChunkSize)
    {
        _maxChunkSize = maxChunkSize;
        return this;
    }
    
    public DownloadBuilder WithMaxConcurrency(int maxConcurrency)
    {
        _maxConcurrency = maxConcurrency;
        return this;
    }
    
    public DownloadBuilder WithFileDestinationType(FileWriterType fileWriterType)
    {
        _fileWriterType = fileWriterType;
        return this;
    }
    
    public DownloadBuilder ConfigureHttpClient(Action<HttpClient> configure)
    {
        configure(_client);
        return this;
    }
    
    public DownloadBuilder OnStarted(Action handler) {
        _onStartedHandlers.Add(handler);
        return this;
    }
    
    public DownloadBuilder OnProgress(Action<IOverallProgress> handler) {
        _onProgressHandler.Add(handler);
        return this;
    }
    
    public DownloadBuilder OnEnded(Action<bool> handler) {
        _onEndedHandler.Add(handler);
        return this;
    }
    
    public DownloadBuilder OnError(Action<Exception> handler) {
        _onErrorHandler.Add(handler);
        return this;
    }
    
    public DownloadBuilder OnFileStarted(Action<IFileInstance> handler) {
        _onFileStartedHandler.Add(handler);
        return this;
    }
    
    public DownloadBuilder OnFileProgress(Action<IFileProgress> handler) {
        _onFileProgressHandler.Add(handler);
        return this;
    }
    
    public DownloadBuilder OnFileEnded(Action<IFileInstance> handler) {
        _onFileEndedHandler.Add(handler);
        return this;
    }
    
    public DownloadBuilder OnFileError(Action<(IFileInstance File, Exception Exception)> handler) {
        _onFileErrorHandler.Add(handler);
        return this;
    }
    
    public DownloadBuilder AddDownload(DownloadDefinition download)
    {
        _downloads.Add(download);
        return this;
    }
    
    public DownloadBuilder AddDownloads(IEnumerable<DownloadDefinition> downloads)
    {
        _downloads.AddRange(downloads);
        return this;
    }
    
    public IDownloadSession Build()
    {
        var session = new DownloadSession(_maxChunkSize, _maxConcurrency, _fileWriterType, _client);
        foreach (var handler in _onStartedHandlers) session.OnStarted += (_, _) => handler();
        foreach (var handler in _onProgressHandler) session.OnProgress += (_, progress) => handler(progress);
        foreach (var handler in _onEndedHandler) session.OnEnded += (_, success) => handler(success);
        foreach (var handler in _onErrorHandler) session.OnError += (_, exception) => handler(exception);
        foreach (var handler in _onFileStartedHandler) session.OnFileStarted += (_, file) => handler(file);
        foreach (var handler in _onFileProgressHandler) session.OnFileProgress += (_, fp) => handler(fp);
        foreach (var handler in _onFileEndedHandler) session.OnFileEnded += (_, file) => handler(file);
        foreach (var handler in _onFileErrorHandler) session.OnFileError += (_, fe) => handler(fe);
        session.AddDownloads(_downloads);
        return session;
    }
}