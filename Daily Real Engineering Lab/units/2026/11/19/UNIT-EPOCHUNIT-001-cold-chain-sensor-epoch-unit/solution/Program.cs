using System;
using System.Collections.Generic;
using System.Globalization;

var now = DateTimeOffset.FromUnixTimeSeconds(1_800_000_000);
var packets = new[]
{
    new Packet("A", "A-17", 1_800_000_000_000, 3.2m),
    new Packet("B", "B-22", 1_800_000_000, 7.8m),
    new Packet("A", "A-18", 1_800_000_060_000, 2.8m)
};
foreach (var p in packets)
{
    var result = Decode(p, now);
    Console.WriteLine($"{p.Gateway}/{p.Id} status={result.Status} timestamp={result.Timestamp} temp={p.Celsius.ToString(CultureInfo.InvariantCulture)}");
}
static Result Decode(Packet packet, DateTimeOffset now)
{
    try
    {
        var timestamp = packet.Gateway switch
        {
            "A" => DateTimeOffset.FromUnixTimeMilliseconds(packet.Epoch),
            "B" => DateTimeOffset.FromUnixTimeSeconds(packet.Epoch),
            _ => throw new ArgumentException("Unknown gateway protocol")
        };
        var age = now - timestamp;
        var status = age < TimeSpan.Zero ? "FUTURE" : age > TimeSpan.FromMinutes(10) ? "STALE" : "ACCEPT";
        return new Result(status, timestamp.ToString("O", CultureInfo.InvariantCulture));
    }
    catch (ArgumentOutOfRangeException)
    {
        return new Result("INVALID", "out-of-range");
    }
}
record Packet(string Gateway, string Id, long Epoch, decimal Celsius);
record Result(string Status, string Timestamp);
