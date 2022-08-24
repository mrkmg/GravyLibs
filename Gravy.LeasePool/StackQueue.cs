using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Gravy.LeasePool;

[SuppressMessage("ReSharper", "UnusedMember.Global")] // Keeping methods for completeness
internal class StackQueue<T> : ICollection<T>, ICollection, IReadOnlyCollection<T>, ISerializable, IDeserializationCallback
{
    private readonly LinkedList<T> _list = new();

    public bool TryGetOldest([NotNullWhen(true)] out T? obj)
    {
        if (_list.Count == 0)
        {
            obj = default;
            return false;
        }
        obj = _list.First.Value!;
        _list.RemoveFirst();
        return true;
    } 
    
    public bool TryGetNewest([NotNullWhen(true)] out T? obj)
    {
        if (_list.Count == 0)
        {
            obj = default;
            return false;
        }
        obj = _list.Last.Value!;
        _list.RemoveLast();
        return true;
    }
    
    public bool TryPeekOldest([NotNullWhen(true)] out T? obj)
    {
        if (_list.Count == 0)
        {
            obj = default;
            return false;
        }
        obj = _list.First.Value!;
        return true;
    }
    
    public bool TryPeekNewest([NotNullWhen(true)] out T? obj)
    {
        if (_list.Count == 0)
        {
            obj = default;
            return false;
        }
        obj = _list.Last.Value!;
        return true;
    }
    
    public void RemoveOldest() => _list.RemoveFirst();
    public void RemoveNewest() => _list.RemoveLast();
    
    public void Add([System.Diagnostics.CodeAnalysis.NotNull] T obj)
    {
        if (obj is null) throw new ArgumentNullException(nameof(obj));
        _list.AddLast(obj);
    }

    public void Clear() => _list.Clear();
    public bool Contains(T item) => _list.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public bool Remove(T item) => _list.Remove(item);
    public void CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);

    public int Count => _list.Count;
    public bool IsSynchronized => ((ICollection)_list).IsSynchronized;
    public object SyncRoot => ((ICollection)_list).SyncRoot;
    public bool IsReadOnly => false;

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();
    public void OnDeserialization(object sender)
    {
        _list.OnDeserialization(sender);
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        _list.GetObjectData(info, context);
    }
}