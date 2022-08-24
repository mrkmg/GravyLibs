using System.Collections.Concurrent;

namespace Gravy.MultiHttp.Internal;

internal class ActionPool
{
    private readonly Thread[] _threads;
    private readonly Thread _monitorThread;
    private readonly TaskCompletionSource _tcs = new();
    private readonly ConcurrentBag<Exception> _exceptions = new();
    
    public Task Task => _tcs.Task;

    public ActionPool(IEnumerable<Action> actions)
    {
        _threads = actions.Select(CreateThread).ToArray();
        _monitorThread = new(MonitorThread);
    }

    public void Start()
    {
        foreach (var thread in _threads) thread.Start();
        _monitorThread.Start();
    }

    private void MonitorThread()
    {
        foreach (var thread in _threads) thread.Join();
        
        if (_exceptions.IsEmpty)
            _tcs.SetResult();
        else if (_exceptions.All(e => e is OperationCanceledException))
            _tcs.SetCanceled();
        else if (_exceptions.Count == 1)
            _tcs.SetException(_exceptions.First());
        else
            _tcs.SetException(_exceptions.Where(e => e is not OperationCanceledException));
    }
    
    private Thread CreateThread(Action action)
    {
        return new(() =>
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
            }
        });
    }
}