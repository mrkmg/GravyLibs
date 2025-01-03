﻿namespace Gravy.MultiHttp.Internal;

/// <summary>
/// A buffer that dynamically resizes based on the time it takes to perform operations.
///
/// Call <see cref="TriggerTick"/> after each operation on the buffer. This will both clear
/// and resize the buffer if needed.
/// </summary>
public struct DynamicallySizedBuffer
{
    private readonly int _minTicksPerOperation;
    private readonly int _maxTicksPerOperation;
    private readonly int _maxSize;
    private readonly int _minSize;
    private int _lastTickCount;
    private int _averageTicks;
    private byte[] _buffer;

    public Memory<byte> Memory => new (_buffer);
    public Span<byte> Span => new (_buffer);
    
    public DynamicallySizedBuffer(int initialSize, int minTicksPerOperation, int maxTicksPerOperation, int minSize, int maxSize)
    {
        _averageTicks = (maxTicksPerOperation + minTicksPerOperation) / 2;
        _minTicksPerOperation = minTicksPerOperation;
        _maxTicksPerOperation = maxTicksPerOperation;
        _maxSize = maxSize;
        _minSize = minSize;
        _lastTickCount = Environment.TickCount;
        _buffer = new byte[initialSize];
    }
    
    /// <summary>
    /// Resizes the buffer if needed. Will clear buffer if resized.
    /// </summary>
    public void TriggerTick()
    {
        _averageTicks = (int) (_averageTicks * 0.75 + (Environment.TickCount - _lastTickCount) * 0.25);
        if (_averageTicks < _minTicksPerOperation && _buffer.Length < _maxSize)
            _buffer = new byte[_buffer.Length * 2];
        else if (_averageTicks > _maxTicksPerOperation && _buffer.Length > _minSize)
            _buffer = new byte[_buffer.Length / 2];
        _lastTickCount = Environment.TickCount;
    }
}