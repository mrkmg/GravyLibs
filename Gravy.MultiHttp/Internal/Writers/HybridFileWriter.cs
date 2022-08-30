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
        lock (_writeLock)
        {
            if (File.Exists(_instance.Definition.DestinationFilePath) && !_instance.Definition.Overwrite) 
                throw new FileExistsException(_instance.Definition.DestinationFilePath);
            
            if (File.Exists(_instance.Definition.DestinationFilePath + ".tmp"))
                File.Delete(_instance.Definition.DestinationFilePath + ".tmp");
        
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
                throw new InvalidOperationException("The stream was disposed before all chunks were written.");
            
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
            if (File.Exists(_instance.Definition.DestinationFilePath))
                if (_instance.Definition.Overwrite)
                    File.Delete(_instance.Definition.DestinationFilePath);
                else
                    throw new FileExistsException(_instance.Definition.DestinationFilePath);
            File.Move(_instance.Definition.DestinationFilePath + ".tmp", _instance.Definition.DestinationFilePath);
        }
    }
    
    public void Dispose()
    {
        _destination?.Dispose();
    }
}

public class FileExistsException : Exception
{
    private readonly string _filePath;

    public FileExistsException(string filePath)
    {
        _filePath = filePath;
    }
    
    public override string Message => $"File {_filePath} already exists.";
}