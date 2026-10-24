using System.Net;
using System.Text.Json;

var port = 5077;
var mode = "reproduce";
for (var i = 0; i < args.Length; i++)
{
    if (args[i] == "--port" && i + 1 < args.Length && int.TryParse(args[++i], out var parsed))
        port = parsed;
    else if (args[i] == "--mode" && i + 1 < args.Length)
        mode = args[++i];
    else
    {
        Console.Error.WriteLine("Usage: --port <integer> --mode reproduce|verify");
        return 2;
    }
}
if (port is < 1 or > 65535 || (mode != "reproduce" && mode != "verify"))
{
    Console.Error.WriteLine("Invalid port or mode.");
    return 2;
}

using var handler = new HttpClientHandler { AllowAutoRedirect = false, UseProxy = false };
using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(8) };
var url = $"http://127.0.0.1:{port}/status";

using var proxiedRequest = new HttpRequestMessage(HttpMethod.Get, url);
proxiedRequest.Headers.Host = "partner.example.test";
proxiedRequest.Headers.TryAddWithoutValidation("X-Forwarded-Proto", "https");
using var proxiedResponse = await client.SendAsync(proxiedRequest);
var proxiedBody = await proxiedResponse.Content.ReadAsStringAsync();
Console.WriteLine($"EDGE_SIM status={(int)proxiedResponse.StatusCode} location={proxiedResponse.Headers.Location} body={proxiedBody}");

using var directRequest = new HttpRequestMessage(HttpMethod.Get, url);
using var directResponse = await client.SendAsync(directRequest);
Console.WriteLine($"DIRECT_HTTP status={(int)directResponse.StatusCode} location={directResponse.Headers.Location}");

var directRedirect = directResponse.StatusCode == HttpStatusCode.TemporaryRedirect
    && directResponse.Headers.Location?.Scheme == Uri.UriSchemeHttps;

var edgeRedirect = proxiedResponse.StatusCode == HttpStatusCode.TemporaryRedirect
    && proxiedResponse.Headers.Location?.Scheme == Uri.UriSchemeHttps
    && proxiedResponse.Headers.Location?.Host == "partner.example.test";

var edgeOk = false;
if (proxiedResponse.StatusCode == HttpStatusCode.OK)
{
    try
    {
        using var json = JsonDocument.Parse(proxiedBody);
        edgeOk = json.RootElement.GetProperty("scheme").GetString() == "https";
    }
    catch (JsonException)
    {
        edgeOk = false;
    }
    catch (KeyNotFoundException)
    {
        edgeOk = false;
    }
}

if (mode == "reproduce" && edgeRedirect && directRedirect)
{
    Console.WriteLine("EXPECTED_REPRODUCED: edge request redirected; direct HTTP redirected.");
    return 0;
}
if (mode == "verify" && edgeOk && directRedirect)
{
    Console.WriteLine("VERIFIED: trusted edge request is 200/https; direct HTTP still redirects.");
    return 0;
}
Console.Error.WriteLine($"FAILED: mode={mode}, edgeRedirect={edgeRedirect}, edgeOk={edgeOk}, directRedirect={directRedirect}");
return 1;
