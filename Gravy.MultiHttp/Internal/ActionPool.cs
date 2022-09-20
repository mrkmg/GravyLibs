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
        foreach (var thread in _threads)
            thread.Join();

        if (_exceptions.IsEmpty)
        {
            _tcs.SetResult();
            return;
        }
        
        var exceptions = _exceptions.Where(e => e is not OperationCanceledException).ToArray();
        
        switch (exceptions.Length)
        {
            case 0:
                _tcs.SetCanceled();
                break;
            case 1:
                _tcs.SetException(exceptions[0]);
                break;
            default:
                _tcs.SetException(exceptions);
                break;
        }
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