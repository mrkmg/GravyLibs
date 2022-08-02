namespace Gravy.MultiHttp.Internal.Writers;

internal class HybridFileWriter : IFileWriter
{
    private readonly FileInstance _instance;
    private readonly Memory<byte>[] _chunks;
    private readonly int[] _chunkOffsets;
    private readonly object _writeLock = new();
    private FileStream? _destination;

    public HybridFileWriter(FileInstance instance)
    {
        _instance = instance;
        _chunks = new Memory<byte>[instance.ChunksInternal.Length];
        _chunkOffsets = new int[instance.ChunksInternal.Length];
    }

    public void StartFile()
    {
        if (File.Exists(_instance.Definition.DestinationFilePath) && !_instance.Definition.Overwrite) 
            throw new("File already exists and overwrite is false");
        
        if (File.Exists(_instance.Definition.DestinationFilePath + ".tmp"))
            File.Delete(_instance.Definition.DestinationFilePath + ".tmp");
        
        lock (_writeLock)
        {
            _destination = File.OpenWrite(_instance.Definition.DestinationFilePath + ".tmp");
        }
    }

    public void StartChunk(int chunkIndex)
    {
        _chunks[chunkIndex] = new byte[_instance.ChunksInternal[chunkIndex].TotalBytes];
        _chunkOffsets[chunkIndex] = 0;
    }

    public void WriteChunk(int chunkIndex, ReadOnlyMemory<byte> buffer)
    {
        buffer.CopyTo(_chunks[chunkIndex][_chunkOffsets[chunkIndex]..]);
        _chunkOffsets[chunkIndex] += buffer.Length;
    }

    public void FinalizeChunk(int chunkIndex)
    {
        
        lock (_writeLock)
        {
            if (_destination is null)
                throw new InvalidOperationException(nameof(_destination));
            
            _destination?.Seek(_instance.ChunksInternal[chunkIndex].StartByte, SeekOrigin.Begin);
            _destination?.Write(_chunks[chunkIndex].Span);
            
            _chunks[chunkIndex] = default;
        }
    }

    public void FinalizeFile()
    {
        lock (_writeLock)
        {
            _destination?.Dispose();
            _destination = null;
        }
        if (_instance.Definition.Overwrite && File.Exists(_instance.Definition.DestinationFilePath))
            File.Delete(_instance.Definition.DestinationFilePath);
        File.Move(_instance.Definition.DestinationFilePath + ".tmp", _instance.Definition.DestinationFilePath);
    }
    
    public void Dispose()
    {
        _destination?.Dispose();
    }
}