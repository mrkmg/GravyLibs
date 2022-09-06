using JetBrains.Annotations;

namespace Gravy.MultiHttp.Interfaces;

[PublicAPI]
public interface ITaskable
{
    /// <summary>
    /// A task which will complete when the taskable is completed. Will error if the taskable errors.
    /// </summary>
    Task CompletionTask { get; }
}