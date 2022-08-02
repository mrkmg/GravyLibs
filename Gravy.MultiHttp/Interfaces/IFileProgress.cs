namespace Gravy.MultiHttp.Interfaces;

public interface IFileProgress : ITrackable
{
    IFileInstance Instance { get; }
    int TotalChunks { get; }
    int CompletedChunks { get; }
    int ActiveChunks { get; }
}