using System.Net;
using System.Text;

namespace Gravy.MultiHttp.Test;

public class TestHttpServer : IDisposable
{
    private readonly HttpListener _listener;
    private readonly Task _listenerTask;

    public TestHttpServer(int port)
    {
        _listener = new();
        _listener.Prefixes.Add($"http://localhost:{port}/");
        _listener.Start();
        _listenerTask = Task.Run(Listen);
    }

    private async Task Listen()
    {
        while (_listener.IsListening)
        {
            var context = await _listener.GetContextAsync();
            switch (context.Request.Url?.AbsolutePath)
            {
                case "/1mb":
                    await HandleRequestSize(context, 1024 * 1024);
                    break;
                case "/10mb":
                    await HandleRequestSize(context, 10 * 1024 * 1024);
                    break;
                case "/100mb":
                    await HandleRequestSize(context, 100 * 1024 * 1024);
                    break;
                default:
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                    break;
            }
        }
    }
    
    private async Task HandleRequestSize(HttpListenerContext context, int size)
    {
        context.Response.StatusCode = 200;
        context.Response.ContentType = "application/octet-stream";
        if (context.Request.HttpMethod == "HEAD")
        {
            context.Response.ContentLength64 = size;
            context.Response.AddHeader("Accept-Ranges", "bytes");
            context.Response.Close();
            return;
        }
        if (context.Request.Headers.Get("Range") is { } range)
        {
            var parts = range.Split('=');
            var rangeParts = parts[1].Split('-');
            var start = int.Parse(rangeParts[0]);
            var end = int.Parse(rangeParts[1]);
            context.Response.StatusCode = 206;
            context.Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{size}");
            context.Response.ContentLength64 = end - start + 1;
            await using var responseStream = context.Response.OutputStream;
            await responseStream.WriteAsync(GetBytes(start, end));
            context.Response.Close();
        }
        else
        {
            context.Response.ContentLength64 = size;
            await using var responseStream = context.Response.OutputStream;
            await responseStream.WriteAsync(GetBytes(0, size - 1));
            context.Response.Close();
        }
    }

    public void Stop()
    {
        _listener.Stop();
        _listener.Close();
    }

    public void Dispose()
    {
        ((IDisposable)_listener).Dispose();
    }

    private byte[] GetBytes(int start, int end)
    {
        var bytes = new byte[end - start + 1];
        for (var i = start; i <= end; i++)
            bytes[i - start] = (byte)(i % 255);
        return bytes;
    }
}