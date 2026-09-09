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

try
{
    for (var id = 1; id <= 3; id++)
    {
        using var response = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, $"/documents/{id}"),
            HttpCompletionOption.ResponseHeadersRead);

        Console.WriteLine($"Request {id}: {(int)response.StatusCode}, X-Document-Id={response.Headers.GetValues("X-Document-Id").Single()}");

        if (id == 3)
        {
            Console.WriteLine($"Request 3 completed: {(int)response.StatusCode}");
        }
    }

    Environment.ExitCode = 0;
}
finally
{
    await app.StopAsync();
}
