using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Gravy.UsingLock;

[PublicAPI]
public class Locked<T> : IDisposable
{
    private T _item;
    private SemaphoreSlim _semaphore;
    
    public Locked(T item)
    {
        _item = item;
        _semaphore = new(1, 1);
    }

    public IBorrowed<T> Borrow(TimeSpan timeout, CancellationToken? cancellationToken = null) 
        => Borrow(timeout.TotalMilliseconds <= int.MaxValue ? (int)timeout.TotalMilliseconds : throw new ArgumentOutOfRangeException(nameof(timeout), "`TotalMilliseconds` must be less than `int.MaxValue`"), cancellationToken);
    public IBorrowed<T> Borrow(CancellationToken token) => Borrow(Timeout.Infinite, token);
    public IBorrowed<T> Borrow(int timeout = Timeout.Infinite, CancellationToken? cancellationToken = null)
    {
        _semaphore.Wait(timeout, cancellationToken ?? CancellationToken.None);
        return new Borrowed(this);
    }
    
    public Task<IBorrowed<T>> BorrowAsync(TimeSpan timeout, CancellationToken? cancellationToken = null) => BorrowAsync((int)timeout.TotalMilliseconds, cancellationToken);
    public Task<IBorrowed<T>> BorrowAsync(CancellationToken token) => BorrowAsync(Timeout.Infinite, token);
    public async Task<IBorrowed<T>> BorrowAsync(int timeout = Timeout.Infinite, CancellationToken? cancellationToken = null)
    {
        await _semaphore.WaitAsync(timeout, cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
        return new Borrowed(this);
    }
    
    private class Borrowed : IBorrowed<T>
    {
        public T Value
        {
            get
            {
                if (_disposed != 0)
                    throw new ObjectDisposedException(nameof(Borrowed));
                return _source._item;
            }
        }
        private readonly Locked<T> _source;
        private int _disposed;
        
        public Borrowed(Locked<T> source)
        {
            _source = source;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
                throw new ObjectDisposedException("Already disposed");
            _source._semaphore.Release();
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}