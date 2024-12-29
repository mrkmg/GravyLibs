namespace Gravy.MultiHttp;

public class DownloadBuilder : IPathedDownloadBuilder
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
    
    private readonly List<IDownloadDefinition> _downloads = new();

    private DownloadBuilder() { }
    
    public static IDownloadBuilder Create() => new DownloadBuilder();

    public static IPathedDownloadBuilder Create(string destinationPath)
        => new DownloadBuilder().WithDestinationPath(destinationPath);

    IDownloadBuilder IDownloadBuilder.WithMaxConcurrency(int maxConcurrency)
        => ((IPathedDownloadBuilder)this).WithMaxConcurrency(maxConcurrency);

    IDownloadBuilder IDownloadBuilder.WithFileDestinationType(FileWriterType fileWriterType)
        => ((IPathedDownloadBuilder)this).WithFileDestinationType(fileWriterType);

    IDownloadBuilder IDownloadBuilder.ConfigureHttpClient(Action<HttpClient> configure)
        => ((IPathedDownloadBuilder)this).ConfigureHttpClient(configure);

    IDownloadBuilder IDownloadBuilder.OnStarted(Action handler)
        => ((IPathedDownloadBuilder)this).OnStarted(handler);

    IDownloadBuilder IDownloadBuilder.OnProgress(Action<IOverallProgress> handler)
        => ((IPathedDownloadBuilder)this).OnProgress(handler);

    IDownloadBuilder IDownloadBuilder.OnEnded(Action<bool> handler)
        => ((IPathedDownloadBuilder)this).OnEnded(handler);

    IDownloadBuilder IDownloadBuilder.OnError(Action<Exception> handler)
        => ((IPathedDownloadBuilder)this).OnError(handler);

    IDownloadBuilder IDownloadBuilder.OnFileStarted(Action<IFileInstance> handler)
        => ((IPathedDownloadBuilder)this).OnFileStarted(handler);

    IDownloadBuilder IDownloadBuilder.OnFileProgress(Action<IFileProgress> handler)
        => ((IPathedDownloadBuilder)this).OnFileProgress(handler);

    IDownloadBuilder IDownloadBuilder.OnFileEnded(Action<IFileInstance> handler)
        => ((IPathedDownloadBuilder)this).OnFileEnded(handler);

    IDownloadBuilder IDownloadBuilder.OnFileError(Action<(IFileInstance File, Exception Exception)> handler)
        => ((IPathedDownloadBuilder)this).OnFileError(handler);

    IDownloadBuilder IDownloadBuilder.AddDownload(IDownloadDefinition download)
        => ((IPathedDownloadBuilder)this).AddDownload(download);

    IDownloadBuilder IDownloadBuilder.AddDownloads(IEnumerable<IDownloadDefinition> downloads)
        => ((IPathedDownloadBuilder)this).AddDownloads(downloads);

    IDownloadBuilder IDownloadBuilder.WithMaxChunkSize(long maxChunkSize)
        => ((IPathedDownloadBuilder)this).WithMaxChunkSize(maxChunkSize);
    
    IDownloadSession IPathedDownloadBuilder.Build()
        => ((IDownloadBuilder)this).Build();


    IPathedDownloadBuilder IPathedDownloadBuilder.WithMaxChunkSize(long maxChunkSize)
    {
        _maxChunkSize = maxChunkSize;
        return this;
    }
    
    IPathedDownloadBuilder IPathedDownloadBuilder.WithMaxConcurrency(int maxConcurrency)
    {
        _maxConcurrency = maxConcurrency;
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.WithFileDestinationType(FileWriterType fileWriterType)
    {
        _fileWriterType = fileWriterType;
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.ConfigureHttpClient(Action<HttpClient> configure)
    {
        configure(_client);
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.OnStarted(Action handler) {
        _onStartedHandlers.Add(handler);
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.OnProgress(Action<IOverallProgress> handler) {
        _onProgressHandler.Add(handler);
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.OnEnded(Action<bool> handler) {
        _onEndedHandler.Add(handler);
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.OnError(Action<Exception> handler) {
        _onErrorHandler.Add(handler);
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.OnFileStarted(Action<IFileInstance> handler) {
        _onFileStartedHandler.Add(handler);
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.OnFileProgress(Action<IFileProgress> handler) {
        _onFileProgressHandler.Add(handler);
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.OnFileEnded(Action<IFileInstance> handler) {
        _onFileEndedHandler.Add(handler);
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.OnFileError(Action<(IFileInstance File, Exception Exception)> handler) {
        _onFileErrorHandler.Add(handler);
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.AddDownload(IDownloadDefinition download)
    {
        _downloads.Add(download);
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.AddDownloads(IEnumerable<IDownloadDefinition> downloads)
    {
        _downloads.AddRange(downloads);
        return this;
    }

    private string? _currentDownloadPath = null;
    public IPathedDownloadBuilder WithDestinationPath(string destinationPath)
    {
        _currentDownloadPath = destinationPath;
        return this;
    }
    
    private void AddDownloadInternal(string url, bool overwrite)
    {
        var uri = new Uri(url);
        if (uri.AbsolutePath.EndsWith("/"))
            throw new ArgumentException("URL does not have a file name", nameof(url));
        var fileName = Path.GetFileName(uri.AbsolutePath);
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("URL does not have a file name", nameof(url));
        _downloads.Add(new DownloadDefinition
        {
            SourceUri = url, 
            DestinationFilePath= Path.Combine(_currentDownloadPath ?? throw new InvalidOperationException("Destination path not set"), fileName),
            Overwrite = overwrite,
        });
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.AddDownload(string url)
    {
        AddDownloadInternal(url, false);
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.AddDownload(string url, bool overwrite)
    {
        AddDownloadInternal(url, overwrite);
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.AddDownloads(IEnumerable<string> urls)
    {
        foreach (var url in urls)
            AddDownloadInternal(url, false);
        return this;
    }

    IPathedDownloadBuilder IPathedDownloadBuilder.AddDownloads(IEnumerable<string> urls, bool overwrite)
    {
        foreach (var url in urls)
            AddDownloadInternal(url, overwrite);
        return this;
    }

    IDownloadSession IDownloadBuilder.Build()
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