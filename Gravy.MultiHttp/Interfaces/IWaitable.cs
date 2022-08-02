namespace Gravy.MultiHttp.Interfaces;

public interface IWaitable
{
    Task WaitAsync();
    Task WaitAsync(CancellationToken token);
}