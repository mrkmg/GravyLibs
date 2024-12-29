using System.Security.Cryptography;
using System.Text;

namespace Gravy.MultiHttp.Test;

public class Tests
{
    private TestHttpServer _server = null!;
    private SHA1 _sha1 = null!;
    private string _tempPath = null!;
    private int _port = -1;
    private string Temp1Mb => Path.Combine(_tempPath, "1mb");
    private string Temp10Mb => Path.Combine(_tempPath, "10mb");
    private string Temp100Mb => Path.Combine(_tempPath, "100mb");

    private string Sha11Calc => Convert.ToHexString(SHA1.HashData(File.ReadAllBytes(Temp1Mb)));
    private string Sha110Calc => Convert.ToHexString(SHA1.HashData(File.ReadAllBytes(Temp10Mb)));
    private string Sha1100Calc => Convert.ToHexString(SHA1.HashData(File.ReadAllBytes(Temp100Mb)));

    private const string Sha11Expected = "5AFE120A4568E461E488452BC52B1BA8CCE7BD46";
    private const string Sha110Expected = "2DBA8152C9F3C844AF441B42856F1E424C555AF6";
    private const string Sha1100Expected = "8C93B3FEFFA0D961645FE5A0656350B2B8F613DF";
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _port = new Random().Next(10000, 20000);
        _server = new(_port);
        _sha1 = SHA1.Create();
        _tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        if (!Directory.Exists(_tempPath))
            Directory.CreateDirectory(_tempPath);
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _server.Stop();
        _server.Dispose();
        _sha1.Dispose();
        if (Directory.Exists(_tempPath))
            Directory.Delete(_tempPath, true);
    }

    [SetUp]
    [TearDown]
    public void ClearFiles()
    {
        if (File.Exists(Temp1Mb))
            File.Delete(Temp1Mb);
        if (File.Exists(Temp10Mb))
            File.Delete(Temp10Mb);
        if (File.Exists(Temp100Mb))
            File.Delete(Temp100Mb);
    }

    /// <summary>
    /// Simple test to ensure that the downloads work.
    /// Will download 1mb, 10mb, and 100mb files.
    /// Uses 1 max concurrency and 1GB max chunk size to ensure that the files are downloaded in one chunk.
    /// </summary>
    [Test]
    public async Task DownloadsWorks()
    {
        var downloader = CreateBaseDownloader()
            .WithMaxConcurrency(1)
            .WithMaxChunkSize(1000 * 1024 * 1024)
            .WithFileDestinationType(FileWriterType.Direct)
            .Build();
        
        await downloader.StartAsync();
        
        AssertFiles();
    }

    private IDownloadBuilder CreateBaseDownloader() =>
        DownloadBuilder.Create(_tempPath)
            .AddDownload($"http://localhost:{_port}/1mb")
            .AddDownload($"http://localhost:{_port}/10mb")
            .AddDownload($"http://localhost:{_port}/100mb");
    
    private void AssertFiles() =>
        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(Temp1Mb), Is.True);
            Assert.That(new FileInfo(Temp1Mb).Length, Is.EqualTo(1024 * 1024));
            Assert.That(Sha11Calc, Is.EqualTo(Sha11Expected));
            
            Assert.That(File.Exists(Temp10Mb), Is.True);
            Assert.That(new FileInfo(Temp10Mb).Length, Is.EqualTo(10 * 1024 * 1024));
            Assert.That(Sha110Calc, Is.EqualTo(Sha110Expected));
            
            Assert.That(File.Exists(Temp100Mb), Is.True);
            Assert.That(new FileInfo(Temp100Mb).Length, Is.EqualTo(100 * 1024 * 1024));
            Assert.That(Sha1100Calc, Is.EqualTo(Sha1100Expected));
        });
}