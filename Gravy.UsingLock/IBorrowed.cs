using System;
using JetBrains.Annotations;

namespace Gravy.UsingLock;

[PublicAPI]
public interface IBorrowed<out T> : IDisposable
{
    T Value { get; }
}