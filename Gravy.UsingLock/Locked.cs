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

    public ILockedItem<T> Borrow(TimeSpan timeout, CancellationToken? cancellationToken = null) 
        => Borrow(timeout.TotalMilliseconds <= int.MaxValue ? (int)timeout.TotalMilliseconds : throw new ArgumentOutOfRangeException(nameof(timeout), "`TotalMilliseconds` must be less than `int.MaxValue`"), cancellationToken);
    public ILockedItem<T> Borrow(CancellationToken token) => Borrow(Timeout.Infinite, token);
    public ILockedItem<T> Borrow(int timeout = Timeout.Infinite, CancellationToken? cancellationToken = null)
    {
        _semaphore.Wait(timeout, cancellationToken ?? CancellationToken.None);
        return new LockedItem(this);
    }
    
    public Task<ILockedItem<T>> BorrowAsync(TimeSpan timeout, CancellationToken? cancellationToken = null) => BorrowAsync((int)timeout.TotalMilliseconds, cancellationToken);
    public Task<ILockedItem<T>> BorrowAsync(CancellationToken token) => BorrowAsync(Timeout.Infinite, token);
    public async Task<ILockedItem<T>> BorrowAsync(int timeout = Timeout.Infinite, CancellationToken? cancellationToken = null)
    {
        await _semaphore.WaitAsync(timeout, cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
        return new LockedItem(this);
    }

    public void Do(Action<T> action) => Do(Timeout.Infinite, CancellationToken.None, action);
    public void Do(TimeSpan timeout, Action<T> action) => Do((int)timeout.TotalMilliseconds, CancellationToken.None, action);
    public void Do(TimeSpan timeout, CancellationToken token, Action<T> action) => Do((int)timeout.TotalMilliseconds, token, action);
    public void Do(CancellationToken token, Action<T> action) => Do(Timeout.Infinite, token, action);
    public void Do(int timeout, Action<T> action) => Do(timeout, CancellationToken.None, action);
    public void Do(int timeout, CancellationToken cancellationToken, Action<T> action)
    {
        using var item = Borrow(timeout, cancellationToken);
        action(item.Value);
    }
    
    public Task DoAsync(Func<T, Task> action) => DoAsync(Timeout.Infinite, CancellationToken.None, action);
    public Task DoAsync(TimeSpan timeout, Func<T, Task> action) => DoAsync((int)timeout.TotalMilliseconds, CancellationToken.None, action);
    public Task DoAsync(TimeSpan timeout, CancellationToken token, Func<T, Task> action) => DoAsync((int)timeout.TotalMilliseconds, token, action);
    public Task DoAsync(CancellationToken token, Func<T, Task> action) => DoAsync(Timeout.Infinite, token, action);
    public Task DoAsync(int timeout, Func<T, Task> action) => DoAsync(timeout, CancellationToken.None, action);
    public async Task DoAsync(int timeout, CancellationToken cancellationToken, Func<T, Task> action)
    {
        using var item = await BorrowAsync(timeout, cancellationToken);
        await action(item.Value).ConfigureAwait(false);
    }
    
    public TReturn Do<TReturn>(Func<T, TReturn> action) => Do(Timeout.Infinite, CancellationToken.None, action);
    public TReturn Do<TReturn>(TimeSpan timeout, Func<T, TReturn> action) => Do((int)timeout.TotalMilliseconds, CancellationToken.None, action);
    public TReturn Do<TReturn>(TimeSpan timeout, CancellationToken token, Func<T, TReturn> action) => Do((int)timeout.TotalMilliseconds, token, action);
    public TReturn Do<TReturn>(CancellationToken token, Func<T, TReturn> action) => Do(Timeout.Infinite, token, action);
    public TReturn Do<TReturn>(int timeout, Func<T, TReturn> action) => Do(timeout, CancellationToken.None, action);
    public TReturn Do<TReturn>(int timeout, CancellationToken cancellationToken, Func<T, TReturn> action)
    {
        using var item = Borrow(timeout, cancellationToken);
        return action(item.Value);
    }
    
    public Task<TReturn> DoAsync<TReturn>(Func<T, Task<TReturn>> action) => DoAsync(Timeout.Infinite, CancellationToken.None, action);
    public Task<TReturn> DoAsync<TReturn>(TimeSpan timeout, Func<T, Task<TReturn>> action) => DoAsync((int)timeout.TotalMilliseconds, CancellationToken.None, action);
    public Task<TReturn> DoAsync<TReturn>(TimeSpan timeout, CancellationToken token, Func<T, Task<TReturn>> action) => DoAsync((int)timeout.TotalMilliseconds, token, action);
    public Task<TReturn> DoAsync<TReturn>(CancellationToken token, Func<T, Task<TReturn>> action) => DoAsync(Timeout.Infinite, token, action);
    public Task<TReturn> DoAsync<TReturn>(int timeout, Func<T, Task<TReturn>> action) => DoAsync(timeout, CancellationToken.None, action);
    public async Task<TReturn> DoAsync<TReturn>(int timeout, CancellationToken cancellationToken, Func<T, Task<TReturn>> action)
    {
        using var item = await BorrowAsync(timeout, cancellationToken);
        return await action(item.Value).ConfigureAwait(false);
    }
    
    private class LockedItem : ILockedItem<T>
    {
        public T Value => _source._item;
        private readonly Locked<T> _source;
        private int _disposed;
        
        public LockedItem(Locked<T> source)
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