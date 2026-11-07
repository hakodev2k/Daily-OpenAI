using System.Net;
using System.Net.Sockets;

public static class EndpointRegistry
{
    public static int CurrentPort { get; set; }
}

public static class ClientFactory
{
    public static HttpClient Create()
    {
        var handler = new SocketsHttpHandler
        {
            UseProxy = false,
            MaxConnectionsPerServer = 1,
            PooledConnectionLifetime = Timeout.InfiniteTimeSpan,
            ConnectCallback = async (context, cancellationToken) =>
            {
                var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
                {
                    NoDelay = true
                };

                await socket.ConnectAsync(IPAddress.Loopback, EndpointRegistry.CurrentPort, cancellationToken);
                return new NetworkStream(socket, ownsSocket: true);
            }
        };

        return new HttpClient(handler, disposeHandler: true);
    }
}
