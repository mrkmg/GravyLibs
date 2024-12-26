# Gravy.LeasePool

A simple, configurable, thread-safe Object Pool. Provides a mechanism for constructing, validating, and disposing of objects on the fly, as well limiting the maximum number of total instantiated object and auto-disposal of stale objects.

Adheres to netstandard2.1

### Usage

```c#
using Gravy.LeasePool;

ILeasePool<Connection> pool = new LeasePool<Connection>(
    new LeasePoolConfiguration<Connection>()
    {
        // Maximum number of leases in the pool
        MaxLeases = 10, 
        
        // Maximum time (in milliseconds) an object can remain idle before it is disposed
        IdleTimeout = 30000, 
        
        // How to create and initialize a new object
        Initializer = () => { 
            var connection = new Connection("hostname", "username", "password");
            connection.Open();
            return connection;
        },
        
        // How to finalize an object
        Finalizer = (connection) => connection.Dispose()
        
        // How to validate an object is okay to lease out
        Validator = (connection) => connection.IsConnected(),
        
        // Called before object is leased
        OnLease = (connection) => connection.StartTransaction(),
        
        // Called after object is returned
        OnReturn = (connection) => connection.EndTransaction(),
    }
);

// Get a connection from the pool, waiting up 
// to 2 seconds for one to become available.
using (var connection = await pool.LeaseAsync(TimeSpan.FromSeconds(2))) {
    // do something with connection
}
```

### Configuration Options

- **MaxLeases** *int* The maximum number of object which can be instantiated at once. Default: -1 (no limit) 
- **IdleTimeout** *int* The maximum amount of time an object can remain idle before it is automatically disposed. Default: -1 (no timeout)
- **Initializer** *Func&lt;T&gt;* A function to create a new instance. Default: `() => Activator.CreateInstance<T>()`
- **Validator** *Func&lt;T, bool&gt;* A function to validate an instance. If this returns false, the object will be disposed. Default: `(instance) => true`
- **OnLease** *Action&lt;T&gt;* A function to execute before an instance is leased. Default: Do nothing
- **OnReturn** *Action&lt;T&gt;* A function to execute after an instance is returned. Default: Do nothing
- **Finalizer** *Action&lt;T&gt;* A function to execute when an instance is disposed. Default: If the object is an `IDisposable`, call `Dispose()` on it.

## License

This package is licensed under the MIT License. See the `LICENSE` file for more details.