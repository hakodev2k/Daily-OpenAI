using System.Text.Json;
const string payload = """
{"vesselId":"FERRY-17","arrivalEpochMs":1793743200000}
""";
var feed = JsonSerializer.Deserialize<Arrival>(payload)!;
try
{
    var actual = ArrivalParser.Parse(feed);
    Console.WriteLine($"ARRIVAL_UTC={actual:O}");
    if (args.Contains("--reproduce")) { Console.Error.WriteLine("NOT_REPRODUCED"); Environment.ExitCode=2; return; }
    var expected = new DateTimeOffset(2026,11,3,20,40,0,TimeSpan.Zero);
    if (actual != expected || actual.ToUnixTimeMilliseconds()!=feed.ArrivalEpochMs || ArrivalParser.Parse(new Arrival("EPOCH",0))!=DateTimeOffset.UnixEpoch)
    { Console.Error.WriteLine("VERIFY_FAILED"); Environment.ExitCode=1; return; }
    Console.WriteLine("VERIFY_PASS");
}
catch (ArgumentOutOfRangeException)
{
    Console.WriteLine("REJECTED: ArgumentOutOfRangeException");
    if (!args.Contains("--reproduce")) Environment.ExitCode=1;
}
public sealed record Arrival(string VesselId,long ArrivalEpochMs);
public static class ArrivalParser
{
    public static DateTimeOffset Parse(Arrival feed) => DateTimeOffset.FromUnixTimeSeconds(feed.ArrivalEpochMs);
}
