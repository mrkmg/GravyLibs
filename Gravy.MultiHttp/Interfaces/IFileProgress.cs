using JetBrains.Annotations;

namespace Gravy.MultiHttp.Interfaces;

[PublicAPI]
public interface IFileProgress : ITrackable
{
    IFileInstance Instance { get; }
    int TotalChunks { get; }
    int CompletedChunks { get; }
    int ActiveChunks { get; }
}