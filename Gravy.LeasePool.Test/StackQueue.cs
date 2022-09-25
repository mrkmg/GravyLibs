using System.Linq;
using NUnit.Framework;

namespace Gravy.LeasePool.Test;

public class StackQueueTests
{
    private StackQueue<int> _fiveStack = default!;
    private StackQueue<int> _emptyStack = default!;

    [SetUp]
    public void Setup()
    {
        _fiveStack = new()
        {
            1, 2, 3, 4, 5,
        };
        _emptyStack = new();
    }
    
    [Test]
    public void Add()
    {
        _fiveStack.Add(6);
        Assert.That(_fiveStack.Count, Is.EqualTo(6));
        Assert.That(_fiveStack, Is.EquivalentTo(new [] { 1, 2, 3, 4, 5, 6 }));
        
        _emptyStack.Add(1);
        Assert.That(_emptyStack.Count, Is.EqualTo(1));
        Assert.That(_emptyStack, Is.EquivalentTo(new [] { 1 }));
    }

    [Test]
    public void TryGetOldest()
    {
        Assert.That(_fiveStack.Count, Is.EqualTo(5));
        Assert.That(_fiveStack.TryGetOldest(out var obj), Is.True);
        Assert.That(obj, Is.EqualTo(1));
        Assert.That(_fiveStack.Count, Is.EqualTo(4));
        Assert.That(_fiveStack, Is.EquivalentTo(new [] { 2, 3, 4, 5 }));
        
        Assert.That(_emptyStack.Count, Is.EqualTo(0));
        Assert.That(_emptyStack.TryGetOldest(out obj), Is.False);
        Assert.That(obj, Is.EqualTo(0));
    }
    
    [Test]
    public void TryGetNewest()
    {
        Assert.That(_fiveStack.Count, Is.EqualTo(5));
        Assert.That(_fiveStack.TryGetNewest(out var obj), Is.True);
        Assert.That(obj, Is.EqualTo(5));
        Assert.That(_fiveStack.Count, Is.EqualTo(4));
        Assert.That(_fiveStack, Is.EquivalentTo(new [] { 1, 2, 3, 4 }));
        
        Assert.That(_emptyStack.Count, Is.EqualTo(0));
        Assert.That(_emptyStack.TryGetNewest(out obj), Is.False);
        Assert.That(obj, Is.EqualTo(0));
    }

    [Test]
    public void TryPeekOldest()
    {
        Assert.That(_fiveStack.Count, Is.EqualTo(5));
        Assert.That(_fiveStack.TryPeekOldest(out var obj), Is.True);
        Assert.That(obj, Is.EqualTo(1));
        Assert.That(_fiveStack.Count, Is.EqualTo(5));
        Assert.That(_fiveStack, Is.EquivalentTo(new [] { 1, 2, 3, 4, 5 }));
        
        Assert.That(_emptyStack.Count, Is.EqualTo(0));
        Assert.That(_emptyStack.TryPeekOldest(out obj), Is.False);
        Assert.That(obj, Is.EqualTo(0));
    }
    
    [Test]
    public void TryPeekNewest()
    {
        Assert.That(_fiveStack.Count, Is.EqualTo(5));
        Assert.That(_fiveStack.TryPeekNewest(out var obj), Is.True);
        Assert.That(obj, Is.EqualTo(5));
        Assert.That(_fiveStack.Count, Is.EqualTo(5));
        Assert.That(_fiveStack, Is.EquivalentTo(new [] { 1, 2, 3, 4, 5 }));
        
        Assert.That(_emptyStack.Count, Is.EqualTo(0));
        Assert.That(_emptyStack.TryPeekNewest(out obj), Is.False);
        Assert.That(obj, Is.EqualTo(0));
    }
    
    [Test]
    public void Many()
    {
        var stack = new StackQueue<int>();
        for (var i = 0; i < 1000; i++)
        {
            stack.Add(i);
        }
        Assert.That(stack.Count, Is.EqualTo(1000));
        Assert.That(stack, Is.EquivalentTo(Enumerable.Range(0, 1000)));
        
        for (var i = 0; i < 1000; i++)
        {
            Assert.That(stack.TryGetOldest(out var obj), Is.True);
            Assert.That(obj, Is.EqualTo(i));
        }
        Assert.That(stack.Count, Is.EqualTo(0));
        Assert.That(stack.TryGetOldest(out _), Is.False);
    }
}