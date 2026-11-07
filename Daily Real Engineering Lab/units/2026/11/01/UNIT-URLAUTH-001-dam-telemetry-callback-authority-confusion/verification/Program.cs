using CallbackGate;
// Independent oracle: do not change tests to make a broken policy pass.
var cases = new (string Name, string Url, bool Expected)[]
{
    ("canonical", "https://hooks.damtelemetry.example/ingest", true),
    ("v2", "https://hooks.damtelemetry.example/ingest/v2", true),
    ("mixed-case", "https://HOOKS.DAMTELEMETRY.EXAMPLE/ingest", true),
    ("query", "https://hooks.damtelemetry.example/ingest?station=R12", true),
    ("explicit-default-port", "https://hooks.damtelemetry.example:443/ingest", true),
    ("host-suffix", "https://hooks.damtelemetry.example.relay.invalid/ingest", false),
    ("userinfo-host", "https://hooks.damtelemetry.example@relay.invalid/ingest", false),
    ("credentials", "https://operator@hooks.damtelemetry.example/ingest", false),
    ("wrong-port", "https://hooks.damtelemetry.example:444/ingest", false),
    ("http", "http://hooks.damtelemetry.example/ingest", false),
    ("wrong-route", "https://hooks.damtelemetry.example/ingest-legacy", false),
    ("root-route", "https://hooks.damtelemetry.example/", false),
    ("fragment", "https://hooks.damtelemetry.example/ingest#private", false),
    ("relative", "/ingest", false),
    ("malformed", "not-a-url", false)
};
var failures = 0;
foreach (var c in cases)
{
    try
    {
        bool actual = CallbackPolicy.IsAllowed(c.Url), pass = actual == c.Expected;
        if (!pass) failures++;
        Console.WriteLine($"{(pass ? "PASS" : "FAIL")} {c.Name}: expected={c.Expected}, actual={actual}");
    }
    catch (Exception ex)
    {
        failures++;
        Console.WriteLine($"FAIL {c.Name}: {ex.GetType().Name}: {ex.Message}");
    }
}
Console.WriteLine($"VERIFICATION_FAILURES={failures}");
return failures == 0 ? 0 : 3;
