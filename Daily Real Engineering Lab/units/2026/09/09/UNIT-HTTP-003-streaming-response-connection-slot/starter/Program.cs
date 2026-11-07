using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

const string BaseUrl = "http://127.0.0.1:5127";

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls(BaseUrl);
var app = builder.Build();

app.MapGet("/documents/{id:int}", async (int id, HttpContext context) =>
{
    context.Response.StatusCode = StatusCodes.Status200OK;
    context.Response.ContentType = "application/octet-stream";
    context.Response.Headers["X-Document-Id"] = id.ToString();

    var chunk = new byte[32 * 1024];
    for (var i = 0; i < 64; i++)
    {
        await context.Response.Body.WriteAsync(chunk, context.RequestAborted);
        await context.Response.Body.FlushAsync(context.RequestAborted);
        await Task.Delay(50, context.RequestAborted);
    }
});

await app.StartAsync();

var handler = new SocketsHttpHandler
{
    MaxConnectionsPerServer = 2
};

using var client = new HttpClient(handler)
{
    BaseAddress = new Uri(BaseUrl)
};

var heldResponses = new List<HttpResponseMessage>();

try
{
    for (var id = 1; id <= 2; id++)
    {
        var response = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, $"/documents/{id}"),
            HttpCompletionOption.ResponseHeadersRead);

        Console.WriteLine($"Request {id}: {(int)response.StatusCode}, X-Document-Id={response.Headers.GetValues("X-Document-Id").Single()}");
        heldResponses.Add(response);
    }

    using var budget = new CancellationTokenSource(TimeSpan.FromMilliseconds(700));

    try
    {
        using var third = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, "/documents/3"),
            HttpCompletionOption.ResponseHeadersRead,
            budget.Token);

        Console.WriteLine($"Request 3 completed: {(int)third.StatusCode}");
        Environment.ExitCode = 0;
    }
    catch (OperationCanceledException) when (budget.IsCancellationRequested)
    {
        Console.WriteLine("SYMPTOM: request 3 did not receive response headers within the 700 ms budget.");
        Environment.ExitCode = 1;
    }
}
finally
{
    foreach (var response in heldResponses)
    {
        response.Dispose();
    }

    await app.StopAsync();
}
