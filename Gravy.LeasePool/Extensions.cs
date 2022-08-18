namespace Gravy.LeasePool;

internal static class Extensions
{
    public static void With<T>(this IThreadSafeUsable<T> usable, Action<T> action) where T : IDisposable
    {
        using var l = usable.Use();
        action(l);
    }
    
    public static void With<T>(this IThreadSafeUsable<T> usable, int millisecondsTimeout, CancellationToken cancellationToken, Action<T> action) where T : IDisposable
    {
        using var l = usable.Use(millisecondsTimeout, cancellationToken);
        action(l);
    }
    
    public static async Task WithAsync<T>(this IThreadSafeUsable<T> usable, Func<T, Task> action) where T : IDisposable
    {
        using var l = await usable.UseAsync();
        await action(l);
    }
    
    public static async Task WithAsync<T>(this IThreadSafeUsable<T> usable, int millisecondsTimeout, CancellationToken cancellationToken, Func<T, Task> action) where T : IDisposable
    {
        using var l = await usable.UseAsync(millisecondsTimeout, cancellationToken);
        await action(l);
    }
}