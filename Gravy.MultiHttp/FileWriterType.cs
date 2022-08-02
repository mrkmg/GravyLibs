namespace Gravy.MultiHttp;

/// <summary>Which type of temporary storage to use while downloading a file.</summary>
public enum FileWriterType
{
    /// <summary>Write each chunk to a temporary file on disk. When the request is complete, combine the temporary files into a single file.</summary>
    /// <remarks>Low cpu and memory usage. High disk usage.</remarks>
    TemporaryFiles,
    
    /// <summary>Store chunks in memory. When a chunk is complete, write it to the destination file.</summary>
    /// <remarks>Recommend Method, balance between speed, memory, and disk usage.</remarks>
    Hybrid,
    
    /// <summary>Store chunks in memory. When all chunks are complete, write them to the destination file.</summary>
    /// <remarks>Low cpu and disk usage, but high memory usage.</remarks>
    InMemory,
    
    /// <summary>Writes data to the destination file directly.</summary>
    /// <remarks>Low memory and cpu usage, but very high disk usage. This is terribly inefficient, and should not be used.</remarks>
    Direct
}