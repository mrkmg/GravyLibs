namespace Gravy.LeasePool;

internal static class Extensions
{
    private class SemaphoreSlimDisposer : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public SemaphoreSlimDisposer(SemaphoreSlim semaphoreSlim)
        {
            _semaphoreSlim = semaphoreSlim;
        }
        
        public void Dispose()
        {
            _semaphoreSlim.Release();
        }
    }
    
    public static IDisposable With(this SemaphoreSlim semaphore, int timeout, int totalTimeout)
    {
        if (!semaphore.Wait(timeout))
            throw new LeaseTimeoutException(totalTimeout);
        return new SemaphoreSlimDisposer(semaphore);
    }
    
    public static async Task<IDisposable> WithAsync(this SemaphoreSlim semaphore, int timeout, int totalTimeout, CancellationToken cancellationToken)
    {
        if (!await semaphore.WaitAsync(timeout, cancellationToken).ConfigureAwait(false))
            throw new LeaseTimeoutException(totalTimeout);
        return new SemaphoreSlimDisposer(semaphore);
    }
    
    public static void WaitForLease(this SemaphoreSlim sem, int start, int timeout)
    {
        if (timeout is -1 or 0)
        {
            if (!sem.Wait(timeout))
            {
                throw new LeaseTimeoutException(timeout);
            }
            return;
        }
        var remaining = timeout - Environment.TickCount - start;
        if (remaining < 0 || !sem.Wait(timeout))
        {
            throw new LeaseTimeoutException(timeout);
        }
    }

    public static async Task WaitForLeaseAsync(this SemaphoreSlim sem, int start, int timeout, CancellationToken innerToken)
    {
        if (timeout is -1 or 0)
        {
            if (!await sem.WaitAsync(timeout, innerToken).ConfigureAwait(false))
            {
                throw new LeaseTimeoutException(timeout);
            }
            return;
        }
        var remaining = timeout - Environment.TickCount - start;
        if (remaining < 0 || !await sem.WaitAsync(timeout, innerToken).ConfigureAwait(false))
        {
            throw new LeaseTimeoutException(timeout);
        }
    }
}