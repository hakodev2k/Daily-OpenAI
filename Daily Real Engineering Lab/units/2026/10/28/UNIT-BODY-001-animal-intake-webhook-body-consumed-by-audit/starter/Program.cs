using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

var mode = args.FirstOrDefault() ?? "--run";
if (mode is not ("--run" or "--reproduce" or "--verify"))
{
    Console.Error.WriteLine("Usage: --run | --reproduce | --verify");
    Environment.ExitCode = 64;
    return;
}

var payloads = new[]
{
    """{"AnimalId":"A-17","Species":"cat"}""",
    """{"AnimalId":"B-23","Species":"dog"}"""
};

var results = new List<(string Expected, string Audit, string Endpoint, int Status)>();
foreach (var payload in payloads)
{
    var context = new DefaultHttpContext();
    context.Request.Method = "POST";
    context.Request.Path = "/intake";
    context.Request.ContentType = "application/json";
    context.Request.Body = new NonSeekableReadStream(
        new MemoryStream(Encoding.UTF8.GetBytes(payload)));
    context.Request.ContentLength = Encoding.UTF8.GetByteCount(payload);

    RequestDelegate endpoint = async ctx =>
    {
        using var reader = new StreamReader(ctx.Request.Body, Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        ctx.Items["endpointPayload"] = body;
        try
        {
            var notice = JsonSerializer.Deserialize<IntakeNotice>(body);
            ctx.Response.StatusCode =
                !string.IsNullOrWhiteSpace(notice?.AnimalId) &&
                !string.IsNullOrWhiteSpace(notice.Species) ? 200 : 422;
        }
        catch (JsonException)
        {
            ctx.Response.StatusCode = 422;
        }
    };

    var middleware = new RequestAuditMiddleware(endpoint);
    await middleware.InvokeAsync(context);
    results.Add((payload, context.Items["auditPayload"] as string ?? "",
        context.Items["endpointPayload"] as string ?? "", context.Response.StatusCode));
    context.Request.Body.Dispose();
}

foreach (var (expected, audit, endpoint, status) in results)
    Console.WriteLine($"status={status} auditBytes={Encoding.UTF8.GetByteCount(audit)} endpointBytes={Encoding.UTF8.GetByteCount(endpoint)} expectedBytes={Encoding.UTF8.GetByteCount(expected)}");

var passed = results.All(r => r.Status == 200 && r.Audit == r.Expected && r.Endpoint == r.Expected);
if (mode == "--run")
{
    Console.WriteLine(passed ? "OBSERVED: payload preserved" : "OBSERVED: payload mismatch");
    return;
}
if (mode == "--reproduce")
{
    Console.WriteLine(passed ? "REPRO_UNEXPECTED_PASS" : "REPRO_FAIL");
    Environment.ExitCode = passed ? 0 : 2;
    return;
}
Console.WriteLine(passed ? "VERIFY_PASS" : "VERIFY_FAIL");
Environment.ExitCode = passed ? 0 : 2;

internal sealed record IntakeNotice(string AnimalId, string Species);

internal sealed class NonSeekableReadStream(Stream inner) : Stream
{
    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }
    public override int Read(byte[] buffer, int offset, int count) =>
        inner.Read(buffer, offset, count);
    public override ValueTask<int> ReadAsync(Memory<byte> buffer,
        CancellationToken cancellationToken = default) =>
        inner.ReadAsync(buffer, cancellationToken);
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count,
        CancellationToken cancellationToken) =>
        inner.ReadAsync(buffer, offset, count, cancellationToken);
    public override void Flush() => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) =>
        throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException();
    protected override void Dispose(bool disposing)
    {
        if (disposing) inner.Dispose();
        base.Dispose(disposing);
    }
}
