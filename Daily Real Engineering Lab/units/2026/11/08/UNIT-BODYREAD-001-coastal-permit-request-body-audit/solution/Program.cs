using System.Text;
using Microsoft.AspNetCore.Http;

var cases = new[]
{
    new Case("valid-permit", """{"permitId":"CP-104","zone":"A3"}""", true),
    new Case("missing-permit", """{"zone":"A3"}""", false)
};
var failures = 0;
foreach (var test in cases)
{
    var context = new DefaultHttpContext();
    context.Request.Method = "POST";
    context.Request.ContentType = "application/json";
    var bytes = Encoding.UTF8.GetBytes(test.Json);
    context.Request.ContentLength = bytes.Length;
    context.Request.Body = new MemoryStream(bytes);
    var audit = new List<string>();

    RequestDelegate endpoint = async ctx =>
    {
        using var reader = new StreamReader(ctx.Request.Body, Encoding.UTF8, leaveOpen: true);
        var json = await reader.ReadToEndAsync();
        ctx.Response.StatusCode = json.Contains("\"permitId\"", StringComparison.Ordinal) ? 202 : 422;
    };
    RequestDelegate pipeline = async ctx =>
    {
        ctx.Request.EnableBuffering(bufferThreshold: 1024, bufferLimit: 65536);
        using var reader = new StreamReader(ctx.Request.Body, Encoding.UTF8, leaveOpen: true);
        var payload = await reader.ReadToEndAsync();
        audit.Add($"audit.bytes={Encoding.UTF8.GetByteCount(payload)}");
        ctx.Request.Body.Position = 0;
        await endpoint(ctx);
    };
    await pipeline(context);
    var expected = test.Accept ? 202 : 422;
    var pass = context.Response.StatusCode == expected;
    Console.WriteLine($"{test.Name}: expected={expected} actual={context.Response.StatusCode} {string.Join(" ", audit)} {(pass ? "PASS" : "FAIL")}");
    if (!pass) failures++;
}
return failures == 0 ? 0 : 1;
record Case(string Name, string Json, bool Accept);
