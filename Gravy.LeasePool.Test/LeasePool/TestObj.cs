using System;
using NUnit.Framework;

namespace Gravy.LeasePool.Test;

public class TestObj : IDisposable
{
    public bool Initialized;
    public bool Disposed;
    public bool Leased;
    public bool Returned;
    public bool Validated;
    public bool Custom;

    public void Dispose()
    {
        Disposed = true;
    }
}

public static class TestObjHelpers {
    public static TestObj Initialize()
    {
        var obj = new TestObj();
        obj.Initialized = true;
        return obj;
    }
    
    public static void OnLease(TestObj obj)
    {
        obj.Leased = true;
    }
    
    public static void OnReturn(TestObj obj)
    {
        obj.Returned = true;
    }
    
    public static void OnFinalize(TestObj obj)
    {
        obj.Disposed = true;
    }
    
    public static bool OnValidate(TestObj obj)
    {
        obj.Validated = true;
        return !obj.Custom;
    }
}