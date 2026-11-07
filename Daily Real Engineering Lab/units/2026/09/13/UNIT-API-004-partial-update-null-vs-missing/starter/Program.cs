using System.Text.Json;

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
var cases = new[]
{
    (Name: "missing", Json: "{}", Expected: "blue"),
    (Name: "explicit-null", Json: "{\"tag\":null}", Expected: (string?)null),
    (Name: "new-value", Json: "{\"tag\":\"green\"}", Expected: "green")
};

var failed = false;
foreach (var test in cases)
{
    var request = JsonSerializer.Deserialize<PatchRequest>(test.Json, options)!;
    var actual = Apply("blue", request);
    var ok = actual == test.Expected;
    Console.WriteLine($"{test.Name}: actual={Show(actual)}, expected={Show(test.Expected)}, result={(ok ? "PASS" : "FAIL")}");
    failed |= !ok;
}

return failed ? 1 : 0;

static string? Apply(string? current, PatchRequest request)
{
    // Investigation note: can this DTO tell absent JSON from explicit null?
    if (request.Tag is not null)
        return request.Tag;
    return current;
}

static string Show(string? value) => value ?? "<null>";

sealed class PatchRequest
{
    public string? Tag { get; set; }
}
