using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

const string token = "lab-token";
var entryUri = new Uri("http://127.0.0.1:5181/start");
var trustedResourceAuthority = new Uri("http://127.0.0.1:5182/");
var resourceUri = new Uri(trustedResourceAuthority, "resource");

var redirectApp = WebApplication.CreateBuilder().Build();
redirectApp.Urls.Add(entryUri.GetLeftPart(UriPartial.Authority));
redirectApp.MapGet("/start", () => Results.Redirect(resourceUri.ToString(), permanent: false));

var resourceApp = WebApplication.CreateBuilder().Build();
resourceApp.Urls.Add(trustedResourceAuthority.GetLeftPart(UriPartial.Authority));
resourceApp.MapGet("/resource", (HttpRequest request) =>
{
    var expected = $"Bearer {token}";
    return string.Equals(request.Headers.Authorization.ToString(), expected, StringComparison.Ordinal)
        ? Results.Ok("document-body")
        : Results.Unauthorized();
});

await redirectApp.StartAsync();
await resourceApp.StartAsync();

try
{
    using var handler = new HttpClientHandler { AllowAutoRedirect = false };
    using var client = new HttpClient(handler);

    using var initialRequest = new HttpRequestMessage(HttpMethod.Get, entryUri);
    initialRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

    using var initialResponse = await client.SendAsync(initialRequest);

    if ((int)initialResponse.StatusCode is < 300 or >= 400 || initialResponse.Headers.Location is null)
    {
        Console.WriteLine($"FinalStatus={initialResponse.StatusCode}");
        return 1;
    }

    var redirectUri = initialResponse.Headers.Location.IsAbsoluteUri
        ? initialResponse.Headers.Location
        : new Uri(entryUri, initialResponse.Headers.Location);

    var trustedAuthority = trustedResourceAuthority.GetLeftPart(UriPartial.Authority);
    var redirectAuthority = redirectUri.GetLeftPart(UriPartial.Authority);

    if (!string.Equals(redirectAuthority, trustedAuthority, StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("RejectedUntrustedRedirect=true");
        return 1;
    }

    using var redirectedRequest = new HttpRequestMessage(HttpMethod.Get, redirectUri);
    redirectedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

    using var finalResponse = await client.SendAsync(redirectedRequest);
    Console.WriteLine($"FinalStatus={finalResponse.StatusCode}");

    return finalResponse.StatusCode == HttpStatusCode.OK ? 0 : 1;
}
finally
{
    await resourceApp.StopAsync();
    await redirectApp.StopAsync();
}
