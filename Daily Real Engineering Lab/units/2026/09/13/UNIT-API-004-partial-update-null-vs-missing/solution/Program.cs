using System.Text.Json;

var cases = new[]
{
    (Name: "missing", Json: "{}", Expected: "blue"),
    (Name: "explicit-null", Json: "{\"tag\":null}", Expected: (string?)null),
    (Name: "new-value", Json: "{\"tag\":\"green\"}", Expected: "green")
};

var failed = false;
foreach (var test in cases)
{
    var actual = Apply("blue", test.Json);
    var ok = actual == test.Expected;
    Console.WriteLine($"{test.Name}: actual={Show(actual)}, expected={Show(test.Expected)}, result={(ok ? "PASS" : "FAIL")}");
    failed |= !ok;
}
return failed ? 1 : 0;

static string? Apply(string? current, string json)
{
    using var document = JsonDocument.Parse(json);
    if (!document.RootElement.TryGetProperty("tag", out var tag))
        return current;

    return tag.ValueKind == JsonValueKind.Null ? null : tag.GetString();
}

static string Show(string? value) => value ?? "<null>";
