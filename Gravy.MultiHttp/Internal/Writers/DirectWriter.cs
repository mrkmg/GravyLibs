namespace Gravy.MultiHttp.Internal.Writers;

internal class DirectWriter : IFileWriter
{
    private readonly FileInstance _instance;
    private readonly object _fileLock = new();
    private readonly long[] _chunkOffsets;
    private FileStream? _stream;
    
    public DirectWriter(FileInstance instance)
    {
        _instance = instance;
        _chunkOffsets = new long[instance.ChunksInternal.Length];
        var totalOffset = 0L;
        for (var i = 0; i < instance.ChunksInternal.Length; i++)
        {
            _chunkOffsets[i] = totalOffset;
            totalOffset += instance.ChunksInternal[i].TotalBytes;
        }
    }

    public void StartFile()
    {
        if (File.Exists(_instance.Definition.DestinationFilePath) && !_instance.Definition.Overwrite) 
            throw new("File already exists and overwrite is false");
        
        if (File.Exists(_instance.Definition.DestinationFilePath + ".tmp"))
            File.Delete(_instance.Definition.DestinationFilePath + ".tmp");
        _stream = File.OpenWrite(_instance.Definition.DestinationFilePath + ".tmp");
    }

    public void StartChunk(int chunkIndex) { }

    public void WriteChunk(int chunkIndex, ReadOnlyMemory<byte> buffer)
    {
        lock (_fileLock)
        {
            _stream?.Seek(_chunkOffsets[chunkIndex], SeekOrigin.Begin);
            _stream?.Write(buffer.Span);
            _chunkOffsets[chunkIndex] += buffer.Length;
        }
    }

    public void FinalizeChunk(int chunkIndex) { }

    public void FinalizeFile()
    {
        _stream?.Dispose();
        _stream = null;
        if (_instance.Definition.Overwrite && File.Exists(_instance.Definition.DestinationFilePath))
            File.Delete(_instance.Definition.DestinationFilePath);
        File.Move(_instance.Definition.DestinationFilePath + ".tmp", _instance.Definition.DestinationFilePath);
    }

    public void Dispose()
    {
        _stream?.Dispose();
    }
}