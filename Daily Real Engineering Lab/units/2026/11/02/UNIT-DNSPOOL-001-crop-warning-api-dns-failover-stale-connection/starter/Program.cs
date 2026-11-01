// Deterministic local model; no real DNS, HTTP sockets or external services.
using System;

internal static class Program
{
    // 0 disables age-based replacement in this policy model.
    private const int ConnectionPolicySeconds = 0;

    private static int Main(string[] args)
    {
        if (args.Length > 1 || (args.Length == 1 &&
            args[0] is not ("--run" or "--reproduce" or "--verify")))
        {
            Console.Error.WriteLine("Usage: --run | --reproduce | --verify");
            return 2;
        }

        var report = new Scenario(ConnectionPolicySeconds).Execute();
        Console.WriteLine($"Summary: beforeSuccess={report.BeforeSuccess}, afterSuccess={report.AfterSuccess}, " +
                          $"afterFailures={report.AfterFailures}, connections={report.Connections}, dnsLookups={report.DnsLookups}");

        if (args.Length == 0 || args[0] == "--run") return 0;

        if (args[0] == "--reproduce")
        {
            bool observed = report.BeforeSuccess == 2 &&
                            report.AfterFailures >= 3 &&
                            report.Connections == 1;
            Console.WriteLine(observed ? "REPRODUCED: requests still target retired region." :
                                         "NOT REPRODUCED: starter symptom absent.");
            return observed ? 0 : 1;
        }

        bool verified = report.BeforeSuccess == 2 &&
                        report.AfterSuccess == 5 &&
                        report.AfterFailures == 0 &&
                        report.Connections <= 5 &&
                        report.DnsLookups <= 5;
        Console.WriteLine(verified ? "VERIFIED: recovery and connection-budget contracts pass." :
                                     "FAILED: recovery or connection-budget contract violated.");
        return verified ? 0 : 1;
    }
}

internal sealed record Report(int BeforeSuccess, int AfterSuccess, int AfterFailures,
                              int Connections, int DnsLookups);

internal sealed class Scenario
{
    private const int CutoverSecond = 60;
    private const int DnsCacheTtlSeconds = 30;
    private readonly int _connectionPolicySeconds;
    private string? _cachedDnsTarget;
    private int _cachedDnsExpiry;
    private string? _connectedTarget;
    private int _connectedAt;
    private int _connections;
    private int _dnsLookups;

    public Scenario(int connectionPolicySeconds)
    {
        if (connectionPolicySeconds < 0) throw new ArgumentOutOfRangeException(nameof(connectionPolicySeconds));
        _connectionPolicySeconds = connectionPolicySeconds;
    }

    public Report Execute()
    {
        int beforeSuccess = 0, afterSuccess = 0, afterFailures = 0;
        foreach (int second in new[] { 0, 25, 61, 80, 110, 145, 180 })
        {
            bool needsConnection = _connectedTarget is null ||
                (_connectionPolicySeconds > 0 && second - _connectedAt >= _connectionPolicySeconds);
            if (needsConnection)
            {
                _connectedTarget = Resolve(second);
                _connectedAt = second;
                _connections++;
            }

            string servingRegion = second < CutoverSecond ? "east" : "west";
            bool accepted = _connectedTarget == servingRegion;
            string status = accepted ? "200" : "503";
            Console.WriteLine($"t={second,3}s dnsActive={servingRegion,-4} connected={_connectedTarget,-4} " +
                              $"status={status} connections={_connections} dnsLookups={_dnsLookups}");

            if (second < CutoverSecond)
            {
                if (accepted) beforeSuccess++;
            }
            else if (accepted) afterSuccess++;
            else afterFailures++;
        }
        return new Report(beforeSuccess, afterSuccess, afterFailures, _connections, _dnsLookups);
    }

    private string Resolve(int second)
    {
        if (_cachedDnsTarget is null || second >= _cachedDnsExpiry)
        {
            _cachedDnsTarget = second < CutoverSecond ? "east" : "west";
            _cachedDnsExpiry = second + DnsCacheTtlSeconds;
            _dnsLookups++;
        }
        return _cachedDnsTarget;
    }
}
