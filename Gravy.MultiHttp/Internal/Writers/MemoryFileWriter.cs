namespace Gravy.MultiHttp.Internal.Writers;

internal class MemoryFileWriter : IFileWriter
{
    private readonly FileInstance _instance;
    private readonly Memory<byte>[] _chunks;
    private readonly int[] _chunkOffsets;

    public MemoryFileWriter(FileInstance instance)
    {
        _instance = instance;
        _chunks = new Memory<byte>[instance.ChunksInternal.Length];
        _chunkOffsets = new int[instance.ChunksInternal.Length];
    }

    public void StartFile()
    {
        if (File.Exists(_instance.Definition.DestinationFilePath) && !_instance.Definition.Overwrite) 
            throw new("File already exists and overwrite is false");
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
    }

    public void FinalizeFile()
    {
        if (File.Exists(_instance.Definition.DestinationFilePath + ".tmp"))
            File.Delete(_instance.Definition.DestinationFilePath + ".tmp");
        
        using (var finalFile = File.OpenWrite(_instance.Definition.DestinationFilePath + ".tmp"))
            foreach (var chunk in _chunks)
                finalFile.Write(chunk.Span);
        
        if (_instance.Definition.Overwrite && File.Exists(_instance.Definition.DestinationFilePath))
            File.Delete(_instance.Definition.DestinationFilePath);
        
        File.Move(_instance.Definition.DestinationFilePath + ".tmp", _instance.Definition.DestinationFilePath);
    }

    public void Dispose() { }
}