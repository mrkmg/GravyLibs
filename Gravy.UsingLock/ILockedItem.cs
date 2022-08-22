using System;

namespace Gravy.UsingLock;

public interface ILockedItem<out T> : IDisposable
{
    T Value { get; }
}