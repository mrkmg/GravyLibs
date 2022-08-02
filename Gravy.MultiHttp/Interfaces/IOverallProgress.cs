namespace Gravy.MultiHttp.Interfaces;

public interface IOverallProgress : ITrackable
{
    int TotalFiles { get; }
    int CompletedFiles { get; }
    int ActiveThreads { get; }
    IEnumerable<IFileInstance> ActiveFiles { get; }
}