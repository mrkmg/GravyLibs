using System;
using NUnit.Framework;

namespace Gravy.LeasePool.Test;

public class DefaultConstruction
{
    private LeasePool<TestObj> _pool = default!;
    
    [SetUp]
    public void Setup()
    {
        _pool = new();
    }
    
    [TearDown]
    public void Teardown()
    {
        try { _pool.Dispose(); } 
        catch (ObjectDisposedException) { /* Ignored */ }
    }
    
    [Test]
    public void GetOneObject()
    {
        using var obj = _pool.Lease();
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj.Value, Is.TypeOf<TestObj>());
    }

    [Test]
    public void GetTwoObjects()
    {
        using var obj1 = _pool.Lease();
        using var obj2 = _pool.Lease();
        Assert.That(obj1, Is.Not.EqualTo(obj2));
    }

    [Test]
    public void DisposesObjects()
    {
        var obj = _pool.Lease();
        Assert.That(obj.Value.Disposed, Is.False);
        obj.Dispose();
        Assert.That(obj.Value.Disposed, Is.False);
        _pool.Dispose();
        Assert.That(obj.Value.Disposed, Is.True);
    }

    [Test]
    public void UsesReturnedObjectForNextLease()
    {
        var obj = _pool.Lease();
        obj.Value.Custom = true;
        obj.Dispose();
        var obj2 = _pool.Lease();
        Assert.That(obj2.Value.Custom, Is.True);
        obj2.Dispose();
    }

    [Test]
    public void ReturnAfterPoolDisposed()
    {
        var obj = _pool.Lease();
        _pool.Dispose();
        Assert.That(() => obj.Dispose(), Throws.TypeOf<ObjectDisposedException>());
    }

    [Test]
    public void CannotReturnLeaseTwice()
    {
        var obj = _pool.Lease();
        obj.Dispose();
        Assert.That(() => obj.Dispose(), Throws.TypeOf<ObjectDisposedException>().With.Message.EqualTo("Lease already disposed.\nObject name: 'ActiveLease'."));
    }
}