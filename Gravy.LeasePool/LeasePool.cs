﻿using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Timer = System.Timers.Timer;

namespace Gravy.LeasePool;

/// <inheritdoc />
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class LeasePool<T> : ILeasePool<T> where T : class
{
    /// <inheritdoc />
    public int AvailableLeases => MaxLeases - _leasesSemaphore?.CurrentCount ?? -1;

    protected readonly int MaxLeases;
    protected readonly int IdleTimeout;

    private Func<T>? InitializerFunc { get; }
    private Func<T, bool>? ValidatorFunc { get; }
    private Action<T>? FinalizerAction { get; }
    private Action<T>? OnLeaseAction { get; }
    private Action<T>? OnReturnAction { get; }

    private readonly ThreadSafeDoubleStack<LeasePoolItem> _objects;
    private readonly SemaphoreSlim? _leasesSemaphore;
    private readonly Timer? _timer;
    
    private bool _isDisposed;

    /// <summary>
    /// Create a new LeasePool for the given type.
    /// </summary>
    /// <param name="maxLeases">The maximum number of total instance of T that can be leased or idle in the pool.</param>
    /// <param name="idleTimeout">
    ///     <para>How long an instance of T can be idle in the pool before it is automatically disposed.</para>
    ///     <para>If set to zero, objects are never kept in a pool, and are disposed immediately when returned to the pool.</para>
    /// </param>
    /// <param name="initializer">
    ///     <para>A factory method that creates an instance of T.</para>
    ///     <para>By default, this calls <see cref="Activator.CreateInstance&lt;T&gt;()" /></para>
    /// </param>
    /// <param name="finalizer">
    ///     <para>A factory method that is called when an instance of T is to be disposed.</para>
    ///     <para>By default, this method checks if the instance is an IDisposable and calls Dispose() on it.</para>
    /// </param>
    /// <param name="validator">
    ///     <para>Validates an instance of T before it is leased from the pool.</para>
    ///     <para>If this returns false, the instance will be disposed and a new instance will be created.</para>
    /// </param>
    /// <param name="onLease">Is executed on an object before it is leased.</param>
    /// <param name="onReturn">Is executed on an object after it is returned to the pool.</param>
    /// <exception cref="ArgumentException"> If maxLeases is 0 or less than -1 (-1 means infinite)</exception>
    /// <exception cref="ArgumentException"> If idleTimeout is 0 or less than -1 (-1 means infinite)</exception>
    public LeasePool(int maxLeases = -1, int idleTimeout = -1, 
                     Func<T>? initializer = null, Action<T>? finalizer = null, 
                     Func<T, bool>? validator = null, 
                     Action<T>? onLease = null, Action<T>? onReturn = null)
    {
        MaxLeases = maxLeases;
        IdleTimeout = idleTimeout;
        InitializerFunc = initializer;
        ValidatorFunc = validator;
        FinalizerAction = finalizer;
        OnLeaseAction = onLease;
        OnReturnAction = onReturn;
        
        if (maxLeases is < -1 or 0)
            throw new ArgumentException("Must be greater than 0 or equal to -1", nameof(maxLeases));
        
        if (idleTimeout is < -1 or 0)
            throw new ArgumentException("Must be greater than 0 or equal to -1", nameof(idleTimeout));
        
        _objects = new();
        
        if (IdleTimeout > 0)
        {
            _timer = new(idleTimeout);
            _timer.Elapsed += (_, _) => Cleanup();
        }
        
        if (maxLeases > 0)
            _leasesSemaphore = new(maxLeases, maxLeases);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LeasePool{T}"/> class.
    /// </summary>
    /// <param name="configuration"></param>
    public LeasePool(LeasePoolConfiguration<T> configuration) : this(
        configuration.MaxLeases,
        configuration.IdleTimeout,
        configuration.Initializer,
        configuration.Finalizer,
        configuration.Validator,
        configuration.OnLease,
        configuration.OnReturn) { }

    /// <inheritdoc />
    public Task<ILease<T>> LeaseAsync() => LeaseAsync(Timeout.Infinite, CancellationToken.None);
    
    /// <inheritdoc />
    public Task<ILease<T>> LeaseAsync(CancellationToken cancellationToken) => LeaseAsync(Timeout.Infinite, cancellationToken);
    
    /// <inheritdoc />
    public Task<ILease<T>> LeaseAsync(TimeSpan timeout) => LeaseAsync(timeout, CancellationToken.None);
    
    /// <inheritdoc />
    public Task<ILease<T>> LeaseAsync(TimeSpan timeout, CancellationToken cancellationToken) 
        => LeaseAsync(
            timeout.TotalMilliseconds is > -1 and <= int.MaxValue 
                ? (int)timeout.TotalMilliseconds 
                : throw new ArgumentOutOfRangeException(nameof(timeout)), 
            cancellationToken);
    
    /// <inheritdoc />
    public Task<ILease<T>> LeaseAsync(int millisecondsTimeout) => LeaseAsync(millisecondsTimeout, CancellationToken.None);
    
    /// <inheritdoc />
    public async Task<ILease<T>> LeaseAsync(int millisecondsTimeout, CancellationToken token)
    {
        var start = Environment.TickCount;
        ValidateCanLease(millisecondsTimeout);
        await TakeLeaseAsync(millisecondsTimeout, token);
        
        while (true)
        {
            var timeout = Environment.TickCount - start;
            if (timeout < 0) throw new LeaseTimeoutException(timeout);
            T? item = null;
            using (var objs = await _objects.UseAsync(timeout, token))
                if (objs.TryGetNewest(out var i))
                    item = i.Object;

            if (item is not null)
            {
                if (!Validate(item))
                {
                    Finalize(item);
                    continue;
                }
            }
            else item = Initialize();
            OnLease(item);
            return new ActiveLease(this, item);
        }
    }

    /// <inheritdoc />
    public ILease<T> Lease() => Lease(Timeout.Infinite);

    /// <inheritdoc />
    public ILease<T> Lease(TimeSpan timeout) => Lease(
        timeout.TotalMilliseconds is > -1 and <= int.MaxValue 
            ? (int)timeout.TotalMilliseconds 
            : throw new ArgumentOutOfRangeException(nameof(timeout)));

    /// <inheritdoc />
    public ILease<T> Lease(int millisecondsTimeout)
    {
        var leaseTask = LeaseAsync(millisecondsTimeout);
        leaseTask.Wait();
        if (leaseTask.IsCompletedSuccessfully)
            return leaseTask.Result;
        throw leaseTask.Exception ?? new Exception("Lease failed for unknown reasons");
    }
    
    /// <summary>
    /// Disposes the pool, and all objects in it.
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Dispose()
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(LeasePool<T>));
        _isDisposed = true;
        
        _leasesSemaphore?.Dispose();
        _timer?.Dispose();
        using (var objs = _objects.Use())
            foreach (var obj in objs) Finalize(obj.Object);
        _objects.Dispose();
        GC.SuppressFinalize(this);
    }

    protected virtual T Initialize() => InitializerFunc?.Invoke() ?? Activator.CreateInstance<T>();
    protected virtual bool Validate(T obj) => ValidatorFunc?.Invoke(obj) ?? true;
    protected virtual void OnLease(T obj) => OnLeaseAction?.Invoke(obj);
    protected virtual void OnReturn(T obj) => OnReturnAction?.Invoke(obj);
    protected virtual void Finalize(T obj) => (FinalizerAction ?? DefaultFinalizer).Invoke(obj);

    private void Return(ActiveLease obj)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(LeasePool<T>));
        
        if (!ReferenceEquals(this, obj.Pool))
            throw new InvalidOperationException("Lease is not from this pool");
        
        Debug.Assert(IdleTimeout > 0);

        OnReturn(obj.Value);
        ReleaseLease();
        _objects.With(objs => objs.Add(new(obj.Value)));
        TriggerTimer();
    }

    private void TriggerTimer()
    {
        if (_timer is not null && !_timer.Enabled)
            _timer.Start();
    }

    private void Cleanup()
    {
        using var objs = _objects.Use();
        while (objs.TryPeekOldest(out var item))
        {
            if (item.LastUsed.AddMilliseconds(IdleTimeout) < DateTime.Now)
                return;
            objs.RemoveOldest();
        }
    }
    
    private async Task TakeLeaseAsync(int millisecondsTimeout, CancellationToken token)
    {
        if (_leasesSemaphore != null) 
            await _leasesSemaphore.WaitAsync(millisecondsTimeout, token).ConfigureAwait(false);
    }

    private void ReleaseLease()
    {
        _leasesSemaphore?.Release();
    }

    private void ValidateCanLease(int millisecondsTimeout)
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(LeasePool<T>));
        
        if (millisecondsTimeout < -1)
            throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
    }
    
    private readonly struct ActiveLease : ILease<T>
    {
        public T Value { get; }
        internal readonly LeasePool<T> Pool;
        
        public ActiveLease(LeasePool<T> pool, T value)
        {
            Pool = pool;
            Value = value;
        }
        
        public void Dispose()
        {
            Pool.Return(this);
        }
    }

    private struct LeasePoolItem
    {
        public readonly T Object;
        public readonly DateTime LastUsed;
        
        public LeasePoolItem(T obj)
        {
            Object = obj;
            LastUsed = DateTime.UtcNow;
        }
    }

    private static void DefaultFinalizer(T obj)
    {
        switch (obj)
        {
            case IDisposable disposable:
                disposable.Dispose();
                break;
            case IAsyncDisposable asyncDisposable:
                asyncDisposable.DisposeAsync().AsTask().Wait();
                break;
        }
    }
}


/// <summary>
/// The exception that is thrown when a lease times out.
/// </summary>
public class LeaseTimeoutException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LeaseTimeoutException"/> class.
    /// </summary>
    /// <param name="timeout">The total length of the timeout</param>
    public LeaseTimeoutException(int timeout) : base($"The timeout of {timeout}ms was exceeded") { }
}