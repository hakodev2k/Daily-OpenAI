using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

const string token = "lab-token";
const string entryUrl = "http://127.0.0.1:5181/start";
const string resourceUrl = "http://127.0.0.1:5182/resource";

var redirectApp = WebApplication.CreateBuilder().Build();
redirectApp.Urls.Add("http://127.0.0.1:5181");
redirectApp.MapGet("/start", () => Results.Redirect(resourceUrl, permanent: false));

var resourceApp = WebApplication.CreateBuilder().Build();
resourceApp.Urls.Add("http://127.0.0.1:5182");
resourceApp.MapGet("/resource", (HttpRequest request) =>
{
    var expected = $"Bearer {token}";
    return string.Equals(request.Headers.Authorization, expected, StringComparison.Ordinal)
        ? Results.Ok("document-body")
        : Results.Unauthorized();
});

await redirectApp.StartAsync();
await resourceApp.StartAsync();

try
{
    using var handler = new HttpClientHandler
    {
        AllowAutoRedirect = true
    };

    using var client = new HttpClient(handler);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    // Investigation note:
    // Compare the request that leaves this client with the request received by the final endpoint.
    using var response = await client.GetAsync(entryUrl);

    Console.WriteLine($"FinalStatus={response.StatusCode}");

    return response.StatusCode == HttpStatusCode.OK ? 0 : 1;
}
finally
{
    await resourceApp.StopAsync();
    await redirectApp.StopAsync();
}
