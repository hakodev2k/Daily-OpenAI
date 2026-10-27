using System.Net;
using System.Net.Sockets;
using System.Text;

var mode = args.FirstOrDefault() ?? "--inspect";

await using var east = new LocalGateway("EAST");
await using var west = new LocalGateway("WEST");
var routing = new RoutingSnapshot(east.Port);

using var handler = new SocketsHttpHandler
{
    UseProxy = false,
    MaxConnectionsPerServer = 1,
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
    PooledConnectionLifetime = TimeSpan.FromMilliseconds(150),
    ConnectCallback = async (_, token) =>
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            await socket.ConnectAsync(IPAddress.Loopback, routing.CurrentPort, token);
            return new NetworkStream(socket, ownsSocket: true);
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }
};

using var http = new HttpClient(handler)
{
    BaseAddress = new Uri("http://transit-gateway.invalid/")
};

var first = await http.GetStringAsync("arrivals");
routing.SwitchTo(west.Port);
await Task.Delay(300); // Chờ thay đổi routing ổn định, không phát request đồng thời.
var second = await http.GetStringAsync("arrivals");
var third = await http.GetStringAsync("arrivals");

Console.WriteLine($"Before cutover: {first}");
Console.WriteLine($"After cutover:  {second}");
Console.WriteLine($"Next request:   {third}");
Console.WriteLine($"Route now points to WEST; connection attempts: {routing.ConnectionAttempts}");

return mode switch
{
    "--reproduce" => Assert(first == "EAST" && second == "EAST",
        "Expected to observe stale routing after cutover."),
    "--verify" => Assert(first == "EAST" && second == "WEST" && third == "WEST",
        "Expected requests after cutover to use WEST."),
    "--inspect" => 0,
    _ => 2
};

static int Assert(bool passed, string failure)
{
    Console.WriteLine(passed ? "PASS" : "FAIL: " + failure);
    return passed ? 0 : 1;
}

sealed class RoutingSnapshot
{
    private int _port;
    private int _connections;

    public RoutingSnapshot(int port) => _port = port;
    public int CurrentPort
    {
        get
        {
            Interlocked.Increment(ref _connections);
            return Volatile.Read(ref _port);
        }
    }
    public int ConnectionAttempts => Volatile.Read(ref _connections);
    public void SwitchTo(int port) => Volatile.Write(ref _port, port);
}

sealed class LocalGateway : IAsyncDisposable
{
    private readonly string _name;
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _stop = new();
    private readonly Task _acceptLoop;

    public int Port { get; }

    public LocalGateway(string name)
    {
        _name = name;
        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();
        Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
        _acceptLoop = AcceptAsync();
    }

    private async Task AcceptAsync()
    {
        while (!_stop.IsCancellationRequested)
        {
            TcpClient socket;
            try { socket = await _listener.AcceptTcpClientAsync(_stop.Token); }
            catch (OperationCanceledException) { break; }
            catch (ObjectDisposedException) when (_stop.IsCancellationRequested) { break; }
            catch (SocketException) when (_stop.IsCancellationRequested) { break; }

            _ = RespondAsync(socket);
        }
    }

    private async Task RespondAsync(TcpClient socket)
    {
        try
        {
            using (socket)
            {
                var stream = socket.GetStream();
                using var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, leaveOpen: true);
                while (!_stop.IsCancellationRequested)
                {
                    var requestLine = await reader.ReadLineAsync(_stop.Token);
                    if (requestLine is null) break;
                    if (requestLine.Length == 0) continue;

                    while (true)
                    {
                        var header = await reader.ReadLineAsync(_stop.Token);
                        if (header is null) return;
                        if (header.Length == 0) break;
                    }

                    var body = Encoding.ASCII.GetBytes(_name);
                    var response = Encoding.ASCII.GetBytes(
                        $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {body.Length}\r\nConnection: keep-alive\r\n\r\n");
                    await stream.WriteAsync(response, _stop.Token);
                    await stream.WriteAsync(body, _stop.Token);
                    await stream.FlushAsync(_stop.Token);
                }
            }
        }
        catch (IOException) { }
        catch (OperationCanceledException) { }
        catch (ObjectDisposedException) { }
    }

    public async ValueTask DisposeAsync()
    {
        _stop.Cancel();
        _listener.Stop();
        try { await _acceptLoop; }
        catch (SocketException) when (_stop.IsCancellationRequested) { }
        _stop.Dispose();
    }
}
