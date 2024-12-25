namespace Gravy.MultiHttp.Internal;

// TODO: Rename
internal class UrlProcessor
{
    private const string HeaderAcceptRangesKey = "Accept-Ranges";
    
    private readonly HttpClient _client;

    internal UrlProcessor(HttpClient client)
    {
        _client = client;
    }

    internal FileInstance ProcessDownload(IDownloadDefinition definition, long maxChunkSize, FileWriterType fileWriterType, CancellationToken? token)
    {
        var (totalBytes, acceptsRange) = GetHeaderData(definition, token ?? CancellationToken.None);
        var numChunksNeeded = acceptsRange && fileWriterType != FileWriterType.DirectSequential ? (int)Math.Ceiling(totalBytes / (double)maxChunkSize) : 1;
        var chunks = MakeChunks(totalBytes, numChunksNeeded);
        return new(Guid.NewGuid(), definition, numChunksNeeded == 1 ? FileWriterType.DirectSequential : fileWriterType, totalBytes, chunks);
    }
    
    private (long totalBytes, bool acceptsRange) GetHeaderData(IDownloadDefinition definition, CancellationToken token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Head, definition.SourceUri);
        using var responseMessage = _client.Send(requestMessage, HttpCompletionOption.ResponseHeadersRead, token);

        if (!responseMessage.IsSuccessStatusCode)
            throw new HttpRequestException($"HEAD failed with code \"{responseMessage.StatusCode}\" for \"{definition.SourceUri}\"", null, responseMessage.StatusCode);

        if (responseMessage.Content.Headers.ContentLength is null)
            throw new MissingContentLengthException(definition.SourceUri);

        var totalBytes = responseMessage.Content.Headers.ContentLength.Value;
        if (totalBytes <= 0)
            throw new MissingContentLengthException(definition.SourceUri);

        var acceptsRange = responseMessage.Headers.Contains(HeaderAcceptRangesKey) &&
                           responseMessage.Headers.GetValues(HeaderAcceptRangesKey).Any(x => x == "bytes");
        return (totalBytes, acceptsRange);
    }
    
    private static ChunkInstance[] MakeChunks(long totalBytes, long numChunksNeeded)
    {
        var chunkSize = totalBytes / numChunksNeeded;
        var chunks = new ChunkInstance[numChunksNeeded];
        var currentChunkIndex = 0;
        while (currentChunkIndex < chunks.Length - 1)
        {
            chunks[currentChunkIndex] = new(currentChunkIndex, currentChunkIndex * chunkSize, (currentChunkIndex + 1) * chunkSize - 1);
            currentChunkIndex++;
        }
        chunks[currentChunkIndex] = new(currentChunkIndex, currentChunkIndex * chunkSize, totalBytes - 1);
        return chunks;
    }
}