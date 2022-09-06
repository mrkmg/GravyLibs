using JetBrains.Annotations;

namespace Gravy.MultiHttp.Interfaces;

[PublicAPI]
public interface IOverallProgress : ITrackable
{
    int TotalFiles { get; }
    int CompletedFiles { get; }
    int ActiveThreads { get; }
    IEnumerable<IFileInstance> ActiveFiles { get; }
}