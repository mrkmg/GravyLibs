using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Gravy.UsingLock;

[PublicAPI]
public static class Extensions
{
    public static void Do<T>(this Locked<T> lockedItem, Action<T> action) => Do(lockedItem, Timeout.Infinite, CancellationToken.None, action);
    public static void Do<T>(this Locked<T> lockedItem, TimeSpan timeout, Action<T> action) => Do(lockedItem, (int)timeout.TotalMilliseconds, CancellationToken.None, action);
    public static void Do<T>(this Locked<T> lockedItem, TimeSpan timeout, CancellationToken token, Action<T> action) => Do(lockedItem, (int)timeout.TotalMilliseconds, token, action);
    public static void Do<T>(this Locked<T> lockedItem, CancellationToken token, Action<T> action) => Do(lockedItem, Timeout.Infinite, token, action);
    public static void Do<T>(this Locked<T> lockedItem, int timeout, Action<T> action) => Do(lockedItem, timeout, CancellationToken.None, action);
    public static void Do<T>(this Locked<T> lockedItem, int timeout, CancellationToken cancellationToken, Action<T> action)
    {
        using var borrowed = lockedItem.Borrow(timeout, cancellationToken);
        action(borrowed.Value);
    }
    
    public static Task DoAsync<T>(this Locked<T> lockedItem, Func<T, Task> action) => DoAsync(lockedItem, Timeout.Infinite, CancellationToken.None, action);
    public static Task DoAsync<T>(this Locked<T> lockedItem, TimeSpan timeout, Func<T, Task> action) => DoAsync(lockedItem, (int)timeout.TotalMilliseconds, CancellationToken.None, action);
    public static Task DoAsync<T>(this Locked<T> lockedItem, TimeSpan timeout, CancellationToken token, Func<T, Task> action) => DoAsync(lockedItem, (int)timeout.TotalMilliseconds, token, action);
    public static Task DoAsync<T>(this Locked<T> lockedItem, CancellationToken token, Func<T, Task> action) => DoAsync(lockedItem, Timeout.Infinite, token, action);
    public static Task DoAsync<T>(this Locked<T> lockedItem, int timeout, Func<T, Task> action) => DoAsync(lockedItem, timeout, CancellationToken.None, action);
    public static async Task DoAsync<T>(this Locked<T> lockedItem, int timeout, CancellationToken cancellationToken, Func<T, Task> action)
    {
        using var borrowed = await lockedItem.BorrowAsync(timeout, cancellationToken);
        await action(borrowed.Value).ConfigureAwait(false);
    }
    
    public static Task DoAsync<T>(this Locked<T> lockedItem, Action<T> action) => DoAsync(lockedItem, Timeout.Infinite, CancellationToken.None, action);
    public static Task DoAsync<T>(this Locked<T> lockedItem, TimeSpan timeout, Action<T> action) => DoAsync(lockedItem, (int)timeout.TotalMilliseconds, CancellationToken.None, action);
    public static Task DoAsync<T>(this Locked<T> lockedItem, TimeSpan timeout, CancellationToken token, Action<T> action) => DoAsync(lockedItem, (int)timeout.TotalMilliseconds, token, action);
    public static Task DoAsync<T>(this Locked<T> lockedItem, CancellationToken token, Action<T> action) => DoAsync(lockedItem, Timeout.Infinite, token, action);
    public static Task DoAsync<T>(this Locked<T> lockedItem, int timeout, Action<T> action) => DoAsync(lockedItem, timeout, CancellationToken.None, action);
    public static async Task DoAsync<T>(this Locked<T> lockedItem, int timeout, CancellationToken cancellationToken, Action<T> action)
    {
        using var borrowed = await lockedItem.BorrowAsync(timeout, cancellationToken);
        action(borrowed.Value);
    }
    
    public static TReturn Do<T, TReturn>(this Locked<T> lockedItem, Func<T, TReturn> action) => Do(lockedItem, Timeout.Infinite, CancellationToken.None, action);
    public static TReturn Do<T, TReturn>(this Locked<T> lockedItem, TimeSpan timeout, Func<T, TReturn> action) => Do(lockedItem, (int)timeout.TotalMilliseconds, CancellationToken.None, action);
    public static TReturn Do<T, TReturn>(this Locked<T> lockedItem, TimeSpan timeout, CancellationToken token, Func<T, TReturn> action) => Do(lockedItem, (int)timeout.TotalMilliseconds, token, action);
    public static TReturn Do<T, TReturn>(this Locked<T> lockedItem, CancellationToken token, Func<T, TReturn> action) => Do(lockedItem, Timeout.Infinite, token, action);
    public static TReturn Do<T, TReturn>(this Locked<T> lockedItem, int timeout, Func<T, TReturn> action) => Do(lockedItem, timeout, CancellationToken.None, action);
    public static TReturn Do<T, TReturn>(this Locked<T> lockedItem, int timeout, CancellationToken cancellationToken, Func<T, TReturn> action)
    {
        using var borrowed = lockedItem.Borrow(timeout, cancellationToken);
        return action(borrowed.Value);
    }
    
    public static Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, Func<T, TReturn> action) => DoAsync(lockedItem, Timeout.Infinite, CancellationToken.None, action);
    public static Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, TimeSpan timeout, Func<T, TReturn> action) => DoAsync(lockedItem, (int)timeout.TotalMilliseconds, CancellationToken.None, action);
    public static Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, TimeSpan timeout, CancellationToken token, Func<T, TReturn> action) => DoAsync(lockedItem, (int)timeout.TotalMilliseconds, token, action);
    public static Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, CancellationToken token, Func<T, TReturn> action) => DoAsync(lockedItem, Timeout.Infinite, token, action);
    public static Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, int timeout, Func<T, TReturn> action) => DoAsync(lockedItem, timeout, CancellationToken.None, action);
    public static async Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, int timeout, CancellationToken cancellationToken, Func<T, TReturn> action)
    {
        using var borrowed = await lockedItem.BorrowAsync(timeout, cancellationToken);
        return action(borrowed.Value);
    }
    
    public static Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, Func<T, Task<TReturn>> action) => DoAsync(lockedItem, Timeout.Infinite, CancellationToken.None, action);
    public static Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, TimeSpan timeout, Func<T, Task<TReturn>> action) => DoAsync(lockedItem, (int)timeout.TotalMilliseconds, CancellationToken.None, action);
    public static Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, TimeSpan timeout, CancellationToken token, Func<T, Task<TReturn>> action) => DoAsync(lockedItem, (int)timeout.TotalMilliseconds, token, action);
    public static Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, CancellationToken token, Func<T, Task<TReturn>> action) => DoAsync(lockedItem, Timeout.Infinite, token, action);
    public static Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, int timeout, Func<T, Task<TReturn>> action) => DoAsync(lockedItem, timeout, CancellationToken.None, action);
    public static async Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, int timeout, CancellationToken cancellationToken, Func<T, Task<TReturn>> action)
    {
        using var borrowed = await lockedItem.BorrowAsync(timeout, cancellationToken);
        return await action(borrowed.Value).ConfigureAwait(false);
    }
}