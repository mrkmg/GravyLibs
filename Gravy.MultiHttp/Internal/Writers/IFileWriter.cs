﻿namespace Gravy.MultiHttp.Internal.Writers;

internal interface IFileWriter : IDisposable
{
    void StartFile();
    void StartChunk(int chunkIndex);
    void WriteChunk(int chunkIndex, ReadOnlyMemory<byte> buffer);
    void FinalizeChunk(int chunkIndex);
    void FinalizeFile();
}