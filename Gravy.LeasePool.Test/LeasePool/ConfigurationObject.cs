using System;
using System.Threading;
using NUnit.Framework;

namespace Gravy.LeasePool.Test;

public class ConfigurationObject
{
    private LeasePool<TestObj> _pool;
    
    [SetUp]
    public void Setup()
    {
        _pool = new (new ()
        {
            MaxLeases = 1,
            IdleTimeout = 100,
            Initializer = TestObjHelpers.Initialize,
            Finalizer = TestObjHelpers.OnFinalize,
            Validator = TestObjHelpers.OnValidate,
            OnLease = TestObjHelpers.OnLease,
            OnReturn = TestObjHelpers.OnReturn,
        });
    }
    
    [TearDown]
    public void Teardown()
    {
        try { _pool.Dispose(); } 
        catch (ObjectDisposedException) { /* Ignored */ }
    }
    
    [Test]
    public void Callbacks()
    {
        var obj = _pool.Lease();
        Assert.That(obj.Value.Initialized, Is.True);
        Assert.That(obj.Value.Leased, Is.True);
        Assert.That(obj.Value.Returned, Is.False);
        Assert.That(obj.Value.Disposed, Is.False);
        Assert.That(obj.Value.Validated, Is.False);
        obj.Dispose();
        Assert.That(obj.Value.Returned, Is.True);
        obj = _pool.Lease();
        Assert.That(obj.Value.Validated, Is.True);
        obj.Dispose();
        _pool.Dispose();
        Assert.That(obj.Value.Disposed, Is.True);
    }

    [Test]
    public void MaxLeases()
    {
        var obj1 = _pool.Lease();
        Assert.That(() => _pool.Lease(1), Throws.InstanceOf<LeaseTimeoutException>());
        obj1.Dispose();
    }

    [Test]
    public void Timeout()
    {
        var obj = _pool.Lease();
        obj.Value.Custom = true;
        obj.Dispose();
        Thread.Sleep(150);
        var obj2 = _pool.Lease();
        Assert.That(obj2.Value.Custom, Is.False);
        obj2.Dispose();
    }
}