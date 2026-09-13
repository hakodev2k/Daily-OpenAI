using System.Text.Json;

var original = "blue";
var request = JsonSerializer.Deserialize<PatchRequest>("{\"tag\":null}")!;
var actual = Apply(original, request);
Console.WriteLine(actual ?? "<null>");
return actual is null ? 0 : 1;

static string? Apply(string? current, PatchRequest request)
{
    if (request.Tag is not null)
        return request.Tag;
    return current;
}

sealed class PatchRequest
{
    public string? Tag { get; set; }
}
