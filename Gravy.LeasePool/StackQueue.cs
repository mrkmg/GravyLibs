using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Gravy.LeasePool;

[SuppressMessage("ReSharper", "UnusedMember.Global")] // Keeping methods for completeness
internal class StackQueue<T> : ICollection<T>
{
    private T[] _data = new T[1];
    private int _head;
    private int _length;
    private int _version;

    private int NextIndex() => (_head + _length) % _data.Length;
    private int LastIndex() => (_head + _length - 1) % _data.Length;
    
    public bool TryGetOldest([NotNullWhen(true)] out T? obj)
    {
        if (!TryPeekOldest(out obj))
            return false;
        RemoveOldest();
        return true;
    } 
    
    public bool TryGetNewest([NotNullWhen(true)] out T? obj)
    {
        if (!TryPeekNewest(out obj))
            return false;
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
        var idx = LastIndex();
        _data[idx] = default!;
        _length--;
        ResetHeadIfNeeded();
        _version++;
        return true;
    }

    public void Add([NotNull] T obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));
        
        if (_length == _data.Length)
            Expand();
        
        var next = NextIndex();
        _data[next] = obj;
        _head = next;
        _length++;
        _version++;
    }

    private void Expand()
    {
        var newArr = new T[_data.Length * 2];
        
        if (_length == 0)
        {
            _data = newArr;
            _head = -1;
            _version++;
            return;
        }
        
        if (_head + _length < _data.Length)
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
        List<T> list = new List<T>();
        _data = newArr;
        _head = 0;
        _version++;
    }

    public void Clear()
    {
        _head = -1;
        _length = 0;
        _version++;
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
        _version++;
        return didRemove;
    }

    public int Count => _data.Length;
     
    public bool IsReadOnly => false;

    private void ResetHeadIfNeeded()
    {
        if (_length == 0) _head = -1;
    }

    public IEnumerator<T> GetEnumerator() => new StackQueueEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
            Current = _stackQueue._data[(_stackQueue._head + _offset++) % _stackQueue._data.Length]!;
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