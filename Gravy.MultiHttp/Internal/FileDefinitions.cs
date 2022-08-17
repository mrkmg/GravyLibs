using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Gravy.MultiHttp.Internal;

internal class FileDefinitions : IDisposable
{
    private readonly object _lock = new();
    private readonly ConcurrentQueue<(FileInstance, ChunkInstance)> _pendingChunks = new();
    internal readonly List<FileInstance> FileInstanceList = new();

    public bool AllFilesCompleted()
    {
        lock (_lock)
            return FileInstanceList.All(f => f.Status == Status.Complete);
    }

    public void AddInstance(FileInstance instance)
    {
        lock (_lock)
        {
            FileInstanceList.Add(instance);
            foreach (var chunk in instance.ChunksInternal) 
                _pendingChunks.Enqueue((instance, chunk));
        }
    }
        
    public bool TryGetWaitingChunkToProcess(
        [NotNullWhen(true)] out FileInstance? fileInstance,
        [NotNullWhen(true)] out ChunkInstance? chunkInstance)
    {
        if (_pendingChunks.TryDequeue(out var fileChunk))
        {
            fileInstance = fileChunk.Item1;
            chunkInstance = fileChunk.Item2;
            return true;
        }
        fileInstance = null;
        chunkInstance = null;
        return false;
    }
    
    public void Dispose()
    {
        foreach (var fileInstance in FileInstanceList)
            fileInstance.Dispose();
    }
}