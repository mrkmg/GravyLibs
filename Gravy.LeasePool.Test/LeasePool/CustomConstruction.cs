using System;
using NUnit.Framework;

namespace Gravy.LeasePool.Test;

public class CustomConstruction
{
    private LeasePool<TestObj> _pool = default!;

    [SetUp]
    public void Setup()
    {
        _pool = new(
            -1, -1, 
            TestObjHelpers.Initialize, 
            TestObjHelpers.OnFinalize, 
            TestObjHelpers.OnValidate, 
            TestObjHelpers.OnLease, 
            TestObjHelpers.OnReturn
        );
    }
    
    [TearDown]
    public void Teardown()
    {
        try { _pool.Dispose(); } 
        catch (ObjectDisposedException) { /* Ignored */ }
    }

    [Test]
    public void CallsAllMethods()
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
    public void IssuesNewObjectIfInvalid()
    {
        var obj = _pool.Lease();
        obj.Value.Custom = true;
        obj.Dispose();
        var obj2 = _pool.Lease();
        Assert.That(obj2.Value.Custom, Is.False);
        Assert.That(obj.Value, Is.Not.EqualTo(obj2.Value));
        obj2.Dispose();
    }
}