using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

var mode = args.FirstOrDefault() ?? "run";
if (mode != "run" && mode != "reproduce" && mode != "verify")
{
    Console.Error.WriteLine("Usage: dotnet run -- [run|reproduce|verify]");
    return 2;
}
var payloadA = """{"eventId":"SHIP-1001","sku":"SKU-42","quantity":2}""";
var payloadB = """{"eventId":"SHIP-1002","sku":"SKU-99","quantity":1}""";
var validA = await ScenarioRunner.SendAsync(payloadA);
var validB = await ScenarioRunner.SendAsync(payloadB);
var malformed = await ScenarioRunner.SendAsync("""{"eventId":""");
var empty = await ScenarioRunner.SendAsync("");
Print("valid-A", validA);
Print("valid-B", validB);
Print("malformed", malformed);
Print("empty", empty);
var baseline = malformed.Status == 400 && empty.Status == 400
    && validA.AuditedBytes == Encoding.UTF8.GetByteCount(payloadA)
    && validB.AuditedBytes == Encoding.UTF8.GetByteCount(payloadB);
var symptom = validA.Status == 400 && validB.Status == 400;
var fixedBehavior = validA.Status == 202 && validA.Body == "accepted:SHIP-1001"
    && validB.Status == 202 && validB.Body == "accepted:SHIP-1002";
var passed = baseline && (mode == "reproduce" ? symptom :
    mode == "verify" ? fixedBehavior : true);
Console.WriteLine("Mode=" + mode + "; Passed=" + passed);
return passed ? 0 : 1;

static void Print(string name, ScenarioResult result) =>
    Console.WriteLine(name + ": status=" + result.Status
        + "; body=" + result.Body + "; auditBytes=" + result.AuditedBytes);

record InboundShipment(string EventId, string Sku, int Quantity);
record ScenarioResult(int Status, string Body, long AuditedBytes);

static class ScenarioRunner
{
    public static async Task<ScenarioResult> SendAsync(string json)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "application/json";
        using var input = new NonSeekableReadStream(Encoding.UTF8.GetBytes(json));
        context.Request.Body = input;
        using var output = new MemoryStream();
        context.Response.Body = output;
        RequestDelegate endpoint = async ctx =>
        {
            try
            {
                var shipment = await JsonSerializer.DeserializeAsync<InboundShipment>(
                    ctx.Request.Body,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web),
                    ctx.RequestAborted);
                if (shipment is null || string.IsNullOrWhiteSpace(shipment.EventId)
                    || string.IsNullOrWhiteSpace(shipment.Sku) || shipment.Quantity <= 0)
                {
                    ctx.Response.StatusCode = 400;
                    await ctx.Response.WriteAsync("invalid-event");
                    return;
                }
                ctx.Response.StatusCode = 202;
                await ctx.Response.WriteAsync("accepted:" + shipment.EventId);
            }
            catch (JsonException)
            {
                ctx.Response.StatusCode = 400;
                await ctx.Response.WriteAsync("invalid-json");
            }
        };
        await new RequestAuditMiddleware(endpoint).InvokeAsync(context);
        output.Position = 0;
        using var reader = new StreamReader(output, Encoding.UTF8, leaveOpen: true);
        var response = await reader.ReadToEndAsync();
        var audited = context.Items.TryGetValue("audit.bytes", out var value)
            ? (long)value! : -1L;
        return new ScenarioResult(context.Response.StatusCode, response, audited);
    }
}

sealed class NonSeekableReadStream : Stream
{
    private readonly MemoryStream _inner;
    public NonSeekableReadStream(byte[] bytes) => _inner = new MemoryStream(bytes, writable: false);
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
        _inner.Read(buffer, offset, count);
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count,
        CancellationToken cancellationToken) =>
        _inner.ReadAsync(buffer, offset, count, cancellationToken);
    public override ValueTask<int> ReadAsync(Memory<byte> buffer,
        CancellationToken cancellationToken = default) =>
        _inner.ReadAsync(buffer, cancellationToken);
    public override void Flush() { }
    public override long Seek(long offset, SeekOrigin origin) =>
        throw new NotSupportedException();
    public override void SetLength(long value) =>
        throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException();
    protected override void Dispose(bool disposing)
    {
        if (disposing) _inner.Dispose();
        base.Dispose(disposing);
    }
}
