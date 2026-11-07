using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

public sealed class LocalPartnerServer : IAsyncDisposable
{
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _shutdown = new();
    private readonly ConcurrentBag<Task> _connections = new();
    private readonly Task _acceptLoop;
    private int _reports, _health;
    public Uri BaseUri { get; }
    public int ReportRequests => Volatile.Read(ref _reports);
    public int HealthRequests => Volatile.Read(ref _health);

    public LocalPartnerServer()
    {
        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();
        BaseUri = new Uri($"http://127.0.0.1:{((IPEndPoint)_listener.LocalEndpoint).Port}/");
        _acceptLoop = AcceptLoopAsync();
    }

    private async Task AcceptLoopAsync()
    {
        try
        {
            while (!_shutdown.IsCancellationRequested)
            {
                var socket = await _listener.AcceptTcpClientAsync(_shutdown.Token);
                _connections.Add(ServeAsync(socket, _shutdown.Token));
            }
        }
        catch (OperationCanceledException) when (_shutdown.IsCancellationRequested) { }
        catch (SocketException) when (_shutdown.IsCancellationRequested) { }
    }

    private async Task ServeAsync(TcpClient socket, CancellationToken token)
    {
        using (socket)
        {
            try
            {
                var stream = socket.GetStream();
                using var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, leaveOpen: true);
                var line = await reader.ReadLineAsync(token);
                if (line is null) return;
                while (true)
                {
                    var header = await reader.ReadLineAsync(token);
                    if (string.IsNullOrEmpty(header)) break;
                }
                if (line.StartsWith("GET /report ", StringComparison.Ordinal))
                {
                    Interlocked.Increment(ref _reports);
                    await stream.WriteAsync(Encoding.ASCII.GetBytes(
                        "HTTP/1.1 200 OK\r\nContent-Length: 4096\r\nX-Export-State: ready\r\nConnection: keep-alive\r\n\r\nX"), token);
                    await stream.FlushAsync(token);
                    await Task.Delay(Timeout.InfiniteTimeSpan, token);
                }
                else if (line.StartsWith("GET /health ", StringComparison.Ordinal))
                {
                    Interlocked.Increment(ref _health);
                    await stream.WriteAsync(Encoding.ASCII.GetBytes(
                        "HTTP/1.1 200 OK\r\nContent-Length: 4\r\nConnection: close\r\n\r\nPONG"), token);
                    await stream.FlushAsync(token);
                }
                else
                {
                    await stream.WriteAsync(Encoding.ASCII.GetBytes(
                        "HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\nConnection: close\r\n\r\n"), token);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { }
            catch (IOException) { }
            catch (SocketException) { }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _shutdown.Cancel();
        _listener.Stop();
        await _acceptLoop;
        await Task.WhenAll(_connections.ToArray());
        _shutdown.Dispose();
    }
}
