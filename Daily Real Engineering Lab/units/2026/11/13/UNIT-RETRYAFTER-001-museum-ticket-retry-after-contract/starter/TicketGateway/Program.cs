using TicketGateway;
using System.Globalization;
var now = new DateTimeOffset(2026, 11, 13, 11, 0, 0, TimeSpan.Zero);
var cases = new (string Name, GatewayReply Reply, TimeSpan Expected)[]
{
    ("delta", new(429, "7"), TimeSpan.FromSeconds(7)),
    ("http-date", new(429, now.AddSeconds(35).ToString("r", CultureInfo.InvariantCulture)), TimeSpan.FromSeconds(35)),
    ("expired-date", new(503, now.AddSeconds(-5).ToString("r", CultureInfo.InvariantCulture)), TimeSpan.Zero),
    ("cap-date", new(429, now.AddSeconds(300).ToString("r", CultureInfo.InvariantCulture)), TimeSpan.FromSeconds(120)),
    ("ok", new(200, "90"), TimeSpan.Zero),
    ("missing", new(503, null), TimeSpan.FromSeconds(1)),
    ("invalid", new(429, "tomorrow"), TimeSpan.FromSeconds(1))
};
int failed=0;
foreach(var test in cases)
{
    var actual=RetryPolicy.Delay(test.Reply,now);
    bool pass=actual==test.Expected;
    Console.WriteLine($"{test.Name}: expected={test.Expected.TotalSeconds}s actual={actual.TotalSeconds}s {(pass?"PASS":"FAIL")}");
    if(!pass)failed++;
}
Console.WriteLine($"RESULT: {cases.Length-failed}/{cases.Length} passed");
return failed==0?0:1;
