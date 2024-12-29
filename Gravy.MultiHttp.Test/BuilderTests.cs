using System.Reflection;

namespace Gravy.MultiHttp.Test;

public class BuilderTests
{
    private TestHttpServer _server;
    private int _port = -1;
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _port = new Random().Next(10000, 20000);
        _server = new(_port);
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _server.Stop();
        _server.Dispose();
    }
    
    [Test]
    public void Defaults()
    {
        var session = DownloadBuilder.Create().Build();
        Assert.Multiple(() =>
        {
            Assert.That(session.MaxConcurrency, Is.EqualTo(4));
            Assert.That(session.MaxChunkSize, Is.EqualTo(20971520));
            Assert.That(session.FileWriterType, Is.EqualTo(FileWriterType.Hybrid));
        });
    }
    
    [Test]
    public void MaxConcurrency()
    {
        var session = DownloadBuilder.Create()
            .WithMaxConcurrency(10)
            .Build();
        
        Assert.That(session.MaxConcurrency, Is.EqualTo(10));
    }
    
    [Test]
    public void MaxChunkSize()
    {
        var session = DownloadBuilder.Create()
            .WithMaxChunkSize(1024 * 1024 * 10)
            .Build();
        
        Assert.That(session.MaxChunkSize, Is.EqualTo(1024 * 1024 * 10));
    }
    
    [Test]
    public void FileDestinationType()
    {
        var session = DownloadBuilder.Create()
            .WithFileDestinationType(FileWriterType.Direct)
            .Build();
        
        Assert.That(session.FileWriterType, Is.EqualTo(FileWriterType.Direct));
    }
    
    [Test]
    public void Files()
    {
        var session = DownloadBuilder.Create("/nullPath/")
            .AddDownload($"http://localhost:{_port}/1mb", false)
            .AddDownload($"http://localhost:{_port}/10mb", true)
            .AddDownload($"http://localhost:{_port}/100mb")
            .Build();
        
        Assert.That(session.Files.Count(), Is.EqualTo(3));
        
        var files = session.Files.OrderByDescending(f => f.Definition.SourceUri).ToList();
        Assert.Multiple(() =>
        {
            Assert.That(files[0].Definition.SourceUri, Is.EqualTo($"http://localhost:{_port}/1mb"));
            Assert.That(files[0].Definition.DestinationFilePath, Is.EqualTo("/nullPath/1mb"));
            Assert.That(files[0].Definition.Overwrite, Is.False);
            
            Assert.That(files[1].Definition.SourceUri, Is.EqualTo($"http://localhost:{_port}/10mb"));
            Assert.That(files[1].Definition.DestinationFilePath, Is.EqualTo("/nullPath/10mb"));
            Assert.That(files[1].Definition.Overwrite, Is.True);
            
            Assert.That(files[2].Definition.SourceUri, Is.EqualTo($"http://localhost:{_port}/100mb"));
            Assert.That(files[2].Definition.DestinationFilePath, Is.EqualTo("/nullPath/100mb"));
            Assert.That(files[2].Definition.Overwrite, Is.False);
        });
    }
    
    [Test]
    public void ConfigureHttpClient()
    {
        var session = DownloadBuilder.Create()
            .ConfigureHttpClient(client => client.Timeout = TimeSpan.FromSeconds(999))
            .Build();
        
        // Generally not recommended to do this, maybe _client should be internal not private.
        var client = session.GetType().GetField("_client", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(session) as HttpClient;
        Assert.That(client?.Timeout, Is.EqualTo(TimeSpan.FromSeconds(999)));
    }
}