using CallbackGate;
var cases = new (string Name, string Url, bool Expected)[]
{
    ("canonical", "https://hooks.damtelemetry.example/ingest", true),
    ("v2", "https://hooks.damtelemetry.example/ingest/v2", true),
    ("mixed-case", "https://HOOKS.DAMTELEMETRY.EXAMPLE/ingest", true),
    ("query", "https://hooks.damtelemetry.example/ingest?station=R12", true),
    ("host-suffix", "https://hooks.damtelemetry.example.relay.invalid/ingest", false),
    ("userinfo-host", "https://hooks.damtelemetry.example@relay.invalid/ingest", false),
    ("wrong-port", "https://hooks.damtelemetry.example:444/ingest", false),
    ("wrong-route", "https://hooks.damtelemetry.example/ingest-legacy", false),
    ("root-route", "https://hooks.damtelemetry.example/", false),
    ("http", "http://hooks.damtelemetry.example/ingest", false)
};
var mismatches = 0;
foreach (var c in cases)
{
    bool actual = CallbackPolicy.IsAllowed(c.Url), pass = actual == c.Expected;
    if (!pass) mismatches++;
    Console.WriteLine($"{(pass ? "PASS" : "FAIL")} {c.Name}: expected={c.Expected}, actual={actual}");
}
Console.WriteLine($"MISMATCHES={mismatches}");
return mismatches == 0 ? 0 : 2;
