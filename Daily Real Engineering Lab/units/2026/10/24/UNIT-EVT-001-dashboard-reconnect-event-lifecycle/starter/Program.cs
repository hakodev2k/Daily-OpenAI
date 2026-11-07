using System;
using System.Collections.Generic;
using System.Linq;

namespace EventReconnectLab;

internal sealed class NotificationFeed
{
    public event Action<string>? Published;
    public void Publish(string id) => Published?.Invoke(id);
}

internal sealed class DashboardSession : IDisposable
{
    private readonly NotificationFeed _feed;
    private bool _connected;
    public List<string> Received { get; } = new();

    public DashboardSession(NotificationFeed feed) => _feed = feed;

    public void Connect()
    {
        if (_connected) return;
        _connected = true;
        _feed.Published += OnPublished;
    }

    public void Disconnect()
    {
        if (!_connected) return;
        _connected = false;
    }

    public void Dispose() => Disconnect();
    private void OnPublished(string id) => Received.Add(id);
}

internal static class Program
{
    private sealed record Case(string Name, int Expected, int Actual)
    {
        public bool Pass => Expected == Actual;
    }

    private static Case Steady()
    {
        var feed = new NotificationFeed();
        using var session = new DashboardSession(feed);
        session.Connect();
        feed.Publish("N-101");
        feed.Publish("N-102");
        return new("steady-session", 2, session.Received.Count);
    }

    private static Case RepeatConnect()
    {
        var feed = new NotificationFeed();
        using var session = new DashboardSession(feed);
        session.Connect();
        session.Connect();
        feed.Publish("N-201");
        return new("repeat-connect", 1, session.Received.Count);
    }

    private static Case Reconnect()
    {
        var feed = new NotificationFeed();
        using var session = new DashboardSession(feed);
        session.Connect();
        session.Disconnect();
        session.Connect();
        feed.Publish("N-301");
        return new("reconnect", 1, session.Received.Count);
    }

    private static Case Offline()
    {
        var feed = new NotificationFeed();
        using var session = new DashboardSession(feed);
        session.Connect();
        session.Disconnect();
        feed.Publish("N-401");
        return new("offline-session", 0, session.Received.Count);
    }

    private static Case Disposed()
    {
        var feed = new NotificationFeed();
        var session = new DashboardSession(feed);
        session.Connect();
        session.Dispose();
        feed.Publish("N-501");
        return new("disposed-session", 0, session.Received.Count);
    }

    public static int Main(string[] args)
    {
        if (args.Length != 1 || (args[0] != "smoke" && args[0] != "reproduce" && args[0] != "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run -- [smoke|reproduce|verify]");
            return 64;
        }

        var cases = new[] { Steady(), RepeatConnect(), Reconnect(), Offline(), Disposed() };
        foreach (var item in cases)
            Console.WriteLine($"{item.Name}: expected={item.Expected}, actual={item.Actual}, {(item.Pass ? "PASS" : "FAIL")}");
        int failures = cases.Count(x => !x.Pass);
        Console.WriteLine($"behavioral-mismatches={failures}");

        return args[0] switch
        {
            "smoke" => 0,
            "reproduce" => failures == 3 && cases[0].Pass && cases[1].Pass ? 0 : 1,
            "verify" => failures == 0 ? 0 : 1,
            _ => 64
        };
    }
}
