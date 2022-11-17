namespace Gravy.MultiHttp.Internal.Writers;

internal class SequentialWriter : IFileWriter
{
    private readonly FileInstance _instance;
    private readonly object _fileLock = new();
    private FileStream? _stream;
    private int _currentChunk = -1;
    
    public SequentialWriter(FileInstance instance)
    {
        _instance = instance;
    }

    public void StartFile()
    {
        if (File.Exists(_instance.Definition.DestinationFilePath) && !_instance.Definition.Overwrite) 
            throw new("File already exists and overwrite is false");
        
        if (File.Exists(_instance.Definition.DestinationFilePath + ".tmp"))
            File.Delete(_instance.Definition.DestinationFilePath + ".tmp");
        _stream = File.OpenWrite(_instance.Definition.DestinationFilePath + ".tmp");
    }

    public void StartChunk(int chunkIndex)
    {
        _currentChunk = chunkIndex;
    }

    public void WriteToChunk(int chunkIndex, ReadOnlyMemory<byte> buffer)
    {
        if (_currentChunk != chunkIndex)
            throw new InvalidOperationException("Cannot write to a chunk that is not the current chunk.");
        
        lock (_fileLock)
        {
            _stream?.Write(buffer.Span);
        }
    }

    public void FinalizeChunk(int chunkIndex)
    {
        _currentChunk = -1;
    }

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