namespace Gravy.MultiHttp.Internal.Writers;

internal class TemporaryFilesWriter : IFileWriter
{
    private readonly FileInstance _instance;
    private readonly FileStream?[] _chunks;

    public TemporaryFilesWriter(FileInstance instance)
    {
        _instance = instance;
        _chunks = new FileStream[instance.ChunksInternal.Length];
    }

    public void StartFile()
    {
        if (File.Exists(_instance.Definition.DestinationFilePath) && !_instance.Definition.Overwrite) 
            throw new("File already exists and overwrite is false");
    }

    public void StartChunk(int chunkIndex)
    {
        if (File.Exists(GetChunkPath(chunkIndex)))
            File.Delete(GetChunkPath(chunkIndex));
        _chunks[chunkIndex] = File.OpenWrite(GetChunkPath(chunkIndex));
    }

    public void WriteToChunk(int chunkIndex, ReadOnlyMemory<byte> buffer)
    {
        _chunks[chunkIndex]?.Write(buffer.Span);
    }

    public void FinalizeChunk(int chunkIndex)
    {
        _chunks[chunkIndex]?.Dispose();
        _chunks[chunkIndex] = null!;
    }

    public void FinalizeFile()
    {
        if (File.Exists(_instance.Definition.DestinationFilePath + ".tmp"))
            File.Delete(_instance.Definition.DestinationFilePath + ".tmp");
        using (var finalFile = File.OpenWrite(_instance.Definition.DestinationFilePath + ".tmp"))
        {
            for(var i = 0; i < _chunks.Length; i++)
            {
                var chunkFile = File.OpenRead(GetChunkPath(i));
                chunkFile.CopyTo(finalFile);
                chunkFile.Dispose();
                File.Delete(GetChunkPath(i));
            }
        }
        
        if (_instance.Definition.Overwrite && File.Exists(_instance.Definition.DestinationFilePath))
            File.Delete(_instance.Definition.DestinationFilePath);
        File.Move(_instance.Definition.DestinationFilePath + ".tmp", _instance.Definition.DestinationFilePath);
    }

    public void Dispose()
    {
        foreach (var chunkWriter in _chunks)
            chunkWriter?.Dispose();
    }
    
    private string GetChunkPath(int chunkIndex)
        => _instance.Definition.DestinationFilePath + "." + chunkIndex;
}