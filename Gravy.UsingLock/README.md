## Gravy.UsingLock

The `Gravy.UsingLock` package provides a convenient way to lock objects using `using` statements, ensuring that resources are properly managed and released. This package includes the `Locked<T>` class and extension methods to simplify synchronous and asynchronous locking operations.

### Usage Examples

**Synchronous Example:**
```csharp
var lockedItem = new Locked<MyClass>(new MyClass());
var ret = lockedItem.Do(item => item.MyMethod());
```

**Asynchronous Example:**
```csharp
var lockedItem = new Locked<MyClass>(new MyClass());
var ret = await lockedItem.DoAsync(async item => await item.MyMethodAsync());
```

### Classes

#### `Locked<T>`

The `Locked<T>` class is a generic class that provides a locking mechanism 
for an object of type `T`. Internally, it uses a `SemaphoreSlim` to control 
access to the object.

**Constructor:**
- `Locked(T item)`: Initializes a new instance of the `Locked<T>` class with the specified item.

**Methods:**
- `IBorrowed<T> Borrow(TimeSpan timeout, CancellationToken? cancellationToken = null)`: Borrows the locked item for a specified timeout period.
- `IBorrowed<T> Borrow(CancellationToken token)`: Borrows the locked item with a cancellation token.
- `IBorrowed<T> Borrow(int timeout = Timeout.Infinite, CancellationToken? cancellationToken = null)`: Borrows the locked item for a specified timeout period with an optional cancellation token.
- `Task<IBorrowed<T>> BorrowAsync(TimeSpan timeout, CancellationToken? cancellationToken = null)`: Asynchronously borrows the locked item for a specified timeout period.
- `Task<IBorrowed<T>> BorrowAsync(CancellationToken token)`: Asynchronously borrows the locked item with a cancellation token.
- `Task<IBorrowed<T>> BorrowAsync(int timeout = Timeout.Infinite, CancellationToken? cancellationToken = null)`: Asynchronously borrows the locked item for a specified timeout period with an optional cancellation token.
- `void Dispose()`: Releases the resources used by the `Locked<T>` instance.

### Interfaces

#### `IBorrowed<T>`

The `IBorrowed<T>` interface represents a borrowed instance of a locked item. It provides access to the locked item and ensures that the item is properly released when disposed.

**Properties:**
- `T Value`: Gets the locked item.

**Methods:**
- `void Dispose()`: Releases the borrowed item.

### Extension Methods

The `Extensions` class provides a set of extension methods for the `Locked<T>` class to simplify locking operations.

**Synchronous Methods:**
- `void Do<T>(this Locked<T> lockedItem, Action<T> action)`: Executes an action on the locked item.
- `void Do<T>(this Locked<T> lockedItem, TimeSpan timeout, Action<T> action)`: Executes an action on the locked item with a timeout.
- `void Do<T>(this Locked<T> lockedItem, TimeSpan timeout, CancellationToken token, Action<T> action)`: Executes an action on the locked item with a timeout and cancellation token.
- `void Do<T>(this Locked<T> lockedItem, CancellationToken token, Action<T> action)`: Executes an action on the locked item with a cancellation token.
- `void Do<T>(this Locked<T> lockedItem, int timeout, Action<T> action)`: Executes an action on the locked item with a timeout.
- `void Do<T>(this Locked<T> lockedItem, int timeout, CancellationToken cancellationToken, Action<T> action)`: Executes an action on the locked item with a timeout and cancellation token.
- `TReturn Do<T, TReturn>(this Locked<T> lockedItem, Func<T, TReturn> action)`: Executes a function on the locked item and returns a result.
- `TReturn Do<T, TReturn>(this Locked<T> lockedItem, TimeSpan timeout, Func<T, TReturn> action)`: Executes a function on the locked item with a timeout and returns a result.
- `TReturn Do<T, TReturn>(this Locked<T> lockedItem, TimeSpan timeout, CancellationToken token, Func<T, TReturn> action)`: Executes a function on the locked item with a timeout and cancellation token, and returns a result.
- `TReturn Do<T, TReturn>(this Locked<T> lockedItem, CancellationToken token, Func<T, TReturn> action)`: Executes a function on the locked item with a cancellation token and returns a result.
- `TReturn Do<T, TReturn>(this Locked<T> lockedItem, int timeout, Func<T, TReturn> action)`: Executes a function on the locked item with a timeout and returns a result.
- `TReturn Do<T, TReturn>(this Locked<T> lockedItem, int timeout, CancellationToken cancellationToken, Func<T, TReturn> action)`: Executes a function on the locked item with a timeout and cancellation token, and returns a result.

**Asynchronous Methods:**
- `Task DoAsync<T>(this Locked<T> lockedItem, Func<T, Task> action)`: Asynchronously executes an action on the locked item.
- `Task DoAsync<T>(this Locked<T> lockedItem, TimeSpan timeout, Func<T, Task> action)`: Asynchronously executes an action on the locked item with a timeout.
- `Task DoAsync<T>(this Locked<T> lockedItem, TimeSpan timeout, CancellationToken token, Func<T, Task> action)`: Asynchronously executes an action on the locked item with a timeout and cancellation token.
- `Task DoAsync<T>(this Locked<T> lockedItem, CancellationToken token, Func<T, Task> action)`: Asynchronously executes an action on the locked item with a cancellation token.
- `Task DoAsync<T>(this Locked<T> lockedItem, int timeout, Func<T, Task> action)`: Asynchronously executes an action on the locked item with a timeout.
- `Task DoAsync<T>(this Locked<T> lockedItem, int timeout, CancellationToken cancellationToken, Func<T, Task> action)`: Asynchronously executes an action on the locked item with a timeout and cancellation token.
- `Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, Func<T, TReturn> action)`: Asynchronously executes a function on the locked item and returns a result.
- `Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, TimeSpan timeout, Func<T, TReturn> action)`: Asynchronously executes a function on the locked item with a timeout and returns a result.
- `Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, TimeSpan timeout, CancellationToken token, Func<T, TReturn> action)`: Asynchronously executes a function on the locked item with a timeout and cancellation token, and returns a result.
- `Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, CancellationToken token, Func<T, TReturn> action)`: Asynchronously executes a function on the locked item with a cancellation token and returns a result.
- `Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, int timeout, Func<T, TReturn> action)`: Asynchronously executes a function on the locked item with a timeout and returns a result.
- `Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, int timeout, CancellationToken cancellationToken, Func<T, TReturn> action)`: Asynchronously executes a function on the locked item with a timeout and cancellation token, and returns a result.
- `Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, Func<T, Task<TReturn>> action)`: Asynchronously executes a function on the locked item and returns a result.
- `Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, TimeSpan timeout, Func<T, Task<TReturn>> action)`: Asynchronously executes a function on the locked item with a timeout and returns a result.
- `Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, TimeSpan timeout, CancellationToken token, Func<T, Task<TReturn>> action)`: Asynchronously executes a function on the locked item with a timeout and cancellation token, and returns a result.
- `Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, CancellationToken token, Func<T, Task<TReturn>> action)`: Asynchronously executes a function on the locked item with a cancellation token and returns a result.
- `Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, int timeout, Func<T, Task<TReturn>> action)`: Asynchronously executes a function on the locked item with a timeout and returns a result.
- `Task<TReturn> DoAsync<T, TReturn>(this Locked<T> lockedItem, int timeout, CancellationToken cancellationToken, Func<T, Task<TReturn>> action)`: Asynchronously executes a function on the locked item with a timeout and cancellation token, and returns a result.

### License

This package is licensed under the MIT License. See the `LICENSE` file for more details.
