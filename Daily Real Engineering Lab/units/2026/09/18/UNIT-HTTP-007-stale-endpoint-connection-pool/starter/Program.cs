sealed class FakeDns
{
    public string CurrentEndpoint { get; private set; } = "10.20.0.11";
    public void Rotate() => CurrentEndpoint = "10.20.0.22";
}

sealed class ConnectionPool
{
    private readonly FakeDns _dns;
    private string? _connectedEndpoint;
    private int _connectionAge;

    // Investigation note: decide what lifecycle this pooled connection should have.
    public int MaxConnectionAge { get; set; } = int.MaxValue;

    public ConnectionPool(FakeDns dns) => _dns = dns;

    public string Send()
    {
        if (_connectedEndpoint is null || _connectionAge >= MaxConnectionAge)
        {
            _connectedEndpoint = _dns.CurrentEndpoint;
            _connectionAge = 0;
        }
        _connectionAge++;
        return _connectedEndpoint;
    }
}

var mode = args.FirstOrDefault() ?? "run";
var dns = new FakeDns();
var pool = new ConnectionPool(dns);

var before = pool.Send();
dns.Rotate();
var after1 = pool.Send();
var after2 = pool.Send();

Console.WriteLine($"before={before}");
Console.WriteLine($"dns-now={dns.CurrentEndpoint}");
Console.WriteLine($"after-1={after1}");
Console.WriteLine($"after-2={after2}");

if (mode == "reproduce")
{
    if (after1 == before && dns.CurrentEndpoint != before)
    {
        Console.Error.WriteLine("REPRODUCED: request path still uses the retired endpoint.");
        Environment.Exit(2);
    }
    Environment.Exit(0);
}

if (mode == "verify")
{
    if (after2 != dns.CurrentEndpoint)
    {
        Console.Error.WriteLine("VERIFY FAILED: endpoint rotation was not observed within the required bound.");
        Environment.Exit(3);
    }
    Console.WriteLine("VERIFY PASSED");
}