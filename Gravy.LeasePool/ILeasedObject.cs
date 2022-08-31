using JetBrains.Annotations;

namespace Gravy.LeasePool;

/// <summary>
/// Represents a leased item. Make sure to call Dispose() when you are done with it.
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public interface ILease<T> : IDisposable
{
    /// <summary>
    /// The leased item.
    /// </summary>
    T Value { get; }
}