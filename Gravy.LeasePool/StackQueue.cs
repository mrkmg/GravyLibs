using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Gravy.LeasePool;

[SuppressMessage("ReSharper", "UnusedMember.Global")] // Keeping methods for completeness
internal class StackQueue<T> : ICollection<T>, ICollection, IReadOnlyCollection<T>
{
    private T[] _data = new T[1];
    private int _head;
    private int _length;
    private int _version;

    private int NextIndex() => (_head + _length) % _data.Length;
    private int LastIndex() => (_head + _length - 1) % _data.Length;
    private int IndexOfOffset(int offset) => (_head + offset) % _data.Length;

    private void ResetHeadIfNeeded()
    {
        if (_length == 0)
            _head = 0;
    }
    
    public bool TryGetOldest([NotNullWhen(true)] out T? obj)
    {
        var version = _version;
        if (!TryPeekOldest(out obj))
            return false;
        if (_version != version)
            throw new InvalidOperationException("Collection was modified");
        RemoveOldest();
        return true;
    } 
    
    public bool TryGetNewest([NotNullWhen(true)] out T? obj)
    {
        var version = _version;
        if (!TryPeekNewest(out obj))
            return false;
        if (_version != version)
            throw new InvalidOperationException("Collection was modified");
        RemoveNewest();
        return true;
    }
    
    public bool TryPeekOldest([NotNullWhen(true)] out T? obj)
    {
        if (_length == 0)
        {
            obj = default;
            return false;
        }
        obj = _data[_head]!;
        return true;
    }
    
    public bool TryPeekNewest([NotNullWhen(true)] out T? obj)
    {
        if (_length == 0)
        {
            obj = default;
            return false;
        }
        obj = _data[LastIndex()]!;
        return true;
    }

    public bool RemoveOldest()
    {
        if (_length == 0)
            return false;
        _version++;
        _data[_head] = default!;
        _head++;
        _head %= _data.Length;
        _length--;
        ResetHeadIfNeeded();
        _version++;
        return true;
    }

    public bool RemoveNewest()
    {
        if (_length == 0)
            return false;
        _version++;
        var idx = LastIndex();
        _data[idx] = default!;
        _length--;
        ResetHeadIfNeeded();
        return true;
    }

    public void Add([NotNull] T obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        if (_length == _data.Length)
            Expand();
        else
            _version++;
        
        var next = NextIndex();
        _data[next] = obj;
        _length++;
    }

    private void Expand()
    {
        var newArr = new T[_data.Length * 2];
        _version++;
        
        if (_length == 0)
        {
            _data = newArr;
            _head = 0;
            return;
        }
        
        if (_head + _length <= _data.Length)
        {
            Array.Copy(_data, _head, newArr, 0, _length);
        }
        else
        {
            var endLen = _data.Length - _head;
            var benLen = _length - endLen;
            Array.Copy(_data, _head, newArr, 0, endLen);
            Array.Copy(_data, 0, newArr, endLen, benLen);
        }
        _data = newArr;
        _head = 0;
    }

    #region InterfaceImplementations

    public void Clear()
    {
        _version++;
        _head = 0;
        _length = 0;
    }

    public bool Contains(T item) => _data.Contains(item);

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (_head + _length > _data.Length) // wrap around
        {
            var endLen = _data.Length - _head;
            var benLen = _length - endLen;
            Array.Copy(_data, _head, array, arrayIndex, endLen);
            Array.Copy(_data, 0, array, arrayIndex + endLen, benLen);
        }
        else
        {
            Array.Copy(_data, _head, array, arrayIndex, _length);
        }
    }

    public bool Remove(T item)
    {
        _version++;
        var newArr = new T[_data.Length];
        var newI = 0;
        var didRemove = false;
        for (var i = 0; i < _length; i++)
        {
            var idx = (_head + i) % _data.Length;
            if (_data[idx]!.Equals(item))
            {
                didRemove = true;
                continue;
            }
            newArr[newI++] = _data[idx]!;
        }
        _length = newI;
        _head = 0;
        return didRemove;
    }
    
    public void CopyTo(Array array, int index)
    {
        if (array is not T[] arr)
            throw new ArgumentException("Array must be of type T[]", nameof(array));
        CopyTo(arr, index);
    }

    public int Count => _length;
    public bool IsSynchronized => false;
    public object SyncRoot => this;
    public bool IsReadOnly => false;

    public IEnumerator<T> GetEnumerator() => new StackQueueEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    private class StackQueueEnumerator : IEnumerator<T>
    {
        private readonly int _version;
        private readonly StackQueue<T> _stackQueue;
        private int _offset;

        public T Current { get; private set; } = default!;
        object IEnumerator.Current => Current!;

        public StackQueueEnumerator(StackQueue<T> stackQueue)
        {
            _version = stackQueue._version;
            _stackQueue = stackQueue;
        }

        public bool MoveNext()
        {
            ValidateVersion();
            if (_offset >= _stackQueue._length)
                return false;
            var idx = _stackQueue.IndexOfOffset(_offset++);
            Current = _stackQueue._data[idx]!;
            return true;
        }

        public void Reset()
        {
            ValidateVersion();
            _offset = 0;
        }

        public void Dispose()
        { }
        
        private void ValidateVersion()
        {
            if (_version != _stackQueue._version)
                throw new InvalidOperationException("Collection was modified");
        }
    }
}