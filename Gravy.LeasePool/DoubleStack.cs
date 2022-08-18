using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Gravy.LeasePool;

internal interface IDoubleStack<T> :
    ICollection<T>,
    ICollection,
    IReadOnlyCollection<T>,
    IDeserializationCallback,
    ISerializable
{
    public bool TryGetOldest([NotNullWhen(true)] out T? obj);
    public bool TryGetNewest([NotNullWhen(true)] out T? obj);
    public bool TryPeekOldest([NotNullWhen(true)] out T? obj);
    public bool TryPeekNewest([NotNullWhen(true)] out T? obj);
    public void RemoveOldest();
    public void RemoveNewest();
}

internal interface IUsedDoubleStack<T> : IDoubleStack<T>, IDisposable { }

internal interface IThreadSafeUsable<T>
{
    public T Use(int millisecondsTimeout = -1, CancellationToken? cancellationToken = null);
    public Task<T> UseAsync(int millisecondsTimeout = -1, CancellationToken? cancellationToken = null);
}

internal class DoubleStack<T> : IDoubleStack<T>
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
    
    public void RemoveOldest()
    {
        _list.RemoveFirst();
    }
    
    public void RemoveNewest()
    {
        _list.RemoveLast();
    }
    
    public void Add(T obj)
    {
        _list.AddLast(obj);
    }

    public void Clear()
    {
        _list.Clear();
    }

    public bool Contains(T item) => _list.Contains(item);

    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item) => _list.Remove(item);

    public void CopyTo(Array array, int index)
    {
        ((ICollection)_list).CopyTo(array, index);
    }

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

internal class ThreadSafeDoubleStack<T> : IDisposable, IThreadSafeUsable<IUsedDoubleStack<T>>
{
    private readonly DoubleStack<T> _stack = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public void Dispose()
    {
        _stack.Clear();
        _semaphore.Dispose();
    }
    
    public IUsedDoubleStack<T> Use(int timeout = -1, CancellationToken? token = null)
    {
        _semaphore.Wait(timeout, token ?? CancellationToken.None);
        return new DisposableDoubleStack(_stack, _semaphore);
    }

    public async Task<IUsedDoubleStack<T>> UseAsync(int timeout = -1, CancellationToken? token = null)
    {
        await _semaphore.WaitAsync(timeout, token ?? CancellationToken.None);
        return new DisposableDoubleStack(_stack, _semaphore);
    }

    private class DisposableDoubleStack : 
        IUsedDoubleStack<T>
    {
        private readonly DoubleStack<T> _stack;
        private readonly SemaphoreSlim _semaphore;

        public DisposableDoubleStack(DoubleStack<T> stack, SemaphoreSlim semaphore)
        {
            _stack = stack;
            _semaphore = semaphore;
        }
    
        public void Dispose() => _semaphore.Release();
        
        public bool TryGetOldest([NotNullWhen(true)] out T? obj) => _stack.TryGetOldest(out obj);
        public bool TryGetNewest([NotNullWhen(true)] out T? obj) => _stack.TryGetNewest(out obj);
        public bool TryPeekOldest([NotNullWhen(true)] out T? obj) => _stack.TryPeekOldest(out obj);
        public bool TryPeekNewest([NotNullWhen(true)] out T? obj) => _stack.TryPeekNewest(out obj);
        public void RemoveOldest() => _stack.RemoveOldest();
        public void RemoveNewest() => _stack.RemoveNewest();
        public IEnumerator<T> GetEnumerator() => _stack.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_stack).GetEnumerator();
        public void Add(T item) => _stack.Add(item);
        public void Clear() => _stack.Clear();
        public bool Contains(T item) => _stack.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => _stack.CopyTo(array, arrayIndex);
        public bool Remove(T item) => _stack.Remove(item);
        public void CopyTo(Array array, int index) => _stack.CopyTo(array, index);
        public void OnDeserialization(object sender) => _stack.OnDeserialization(sender);
        public void GetObjectData(SerializationInfo info, StreamingContext context) => _stack.GetObjectData(info, context);

        public bool IsSynchronized => _stack.IsSynchronized;
        public object SyncRoot => _stack.SyncRoot;
        int ICollection.Count => _stack.Count;
        int ICollection<T>.Count => _stack.Count;
        int IReadOnlyCollection<T>.Count => _stack.Count;
        public bool IsReadOnly => _stack.IsReadOnly;
    }
}