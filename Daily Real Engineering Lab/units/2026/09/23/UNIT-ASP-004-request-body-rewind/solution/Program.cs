using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
const string secret = "lab-secret";

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/webhooks/payment")
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var auditBody = await reader.ReadToEndAsync();
        Console.WriteLine($"AUDIT bytes={Encoding.UTF8.GetByteCount(auditBody)}");
        context.Request.Body.Position = 0;
    }
    await next();
});

app.MapPost("/webhooks/payment", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
    var body = await reader.ReadToEndAsync();
    var supplied = request.Headers["X-Lab-Signature"].ToString();
    var expected = Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(body)));
    return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(supplied), Encoding.UTF8.GetBytes(expected))
        ? Results.Ok(new { accepted = true })
        : Results.BadRequest(new { accepted = false, bodyLength = body.Length });
});

app.Run("http://localhost:5084");