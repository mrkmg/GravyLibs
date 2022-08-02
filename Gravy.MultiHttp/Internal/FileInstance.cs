using Gravy.MultiHttp.Internal.Writers;

namespace Gravy.MultiHttp.Internal;

internal class FileInstance : IFileInstance, IDisposable
{
    public Guid Id { get; }
    public IDownloadDefinition Definition { get; }
    public long TotalBytes { get; }
    public Status Status { get; internal set; }
    
    public long CompletedBytes => Progress.CompletedBytes;
    public double CurrentBytesPerSecond => Progress.CurrentBytesPerSecond;
    public double AverageBytesPerSecond => Progress.AverageBytesPerSecond;
    public double OverallBytesPerSecond => Progress.OverallBytesPerSecond;
    public IReadOnlyList<IChunkInstance> Chunks => ChunksInternal;
    public int ElapsedMilliseconds => Progress.ElapsedMilliseconds;

    internal readonly TaskCompletionSource CompletionSource = new();

    internal IFileWriter Writer { get; }
    internal readonly ChunkInstance[] ChunksInternal;
    internal readonly object LockObject = new();
    internal FileProgress Progress;

    public FileInstance(Guid id, IDownloadDefinition definition, FileWriterType writerType, long totalBytes, ChunkInstance[] chunks)
    {
        Id = id;
        Status = Status.Waiting;
        Definition = definition;
        TotalBytes = totalBytes;
        ChunksInternal = chunks;
        Progress = new(this, totalBytes, chunks.Length);
        Writer = writerType switch
        {
            FileWriterType.TemporaryFiles => new TemporaryFilesWriter(this),
            FileWriterType.Hybrid => new HybridFileWriter(this),
            FileWriterType.InMemory => new MemoryFileWriter(this),
            FileWriterType.Direct => new DirectWriter(this),
            _ => throw new ArgumentOutOfRangeException(nameof(writerType), writerType, null)
        };
    }

    public async Task WaitAsync()
        => await CompletionSource.Task.ConfigureAwait(false);

    public async Task WaitAsync(CancellationToken token)
        => await CompletionSource.Task.WaitAsync(token).ConfigureAwait(false);

    public void Dispose()
        => Writer.Dispose();

}