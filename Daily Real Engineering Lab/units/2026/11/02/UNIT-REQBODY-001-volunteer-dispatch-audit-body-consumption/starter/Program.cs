using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

var mode = args.FirstOrDefault() ?? "--demo";
var observations = await DispatchProbe.RunAsync();

Console.WriteLine("Valid dispatch: HTTP " + observations.ValidStatus + " | " + observations.ValidBody);
Console.WriteLine("Malformed dispatch: HTTP " + observations.MalformedStatus);
Console.WriteLine("Audit records: " + observations.AuditEntries.Count);

if (mode == "--reproduce")
{
    // A reproduction succeeds only when the intended original symptom appears.
    bool reproduced = observations.ValidStatus == 400
        && observations.MalformedStatus == 400
        && observations.AuditEntries.Count == 2
        && observations.AuditEntries[0].CharacterCount > 0;
    Console.WriteLine(reproduced ? "REPRODUCED: valid payload rejected after audit." : "NOT REPRODUCED");
    return reproduced ? 0 : 1;
}

if (mode == "--verify")
{
    bool validAccepted = observations.ValidStatus == 200
        && observations.ValidBody == "accepted:V-104";
    bool malformedRejected = observations.MalformedStatus == 400;
    bool auditPreserved = observations.AuditEntries.Count == 2
        && observations.AuditEntries.All(x => x.CharacterCount > 0)
        && observations.AuditEntries[0].Sha256 != observations.AuditEntries[1].Sha256;
    Console.WriteLine("Valid accepted: " + validAccepted);
    Console.WriteLine("Malformed rejected: " + malformedRejected);
    Console.WriteLine("Audit preserved: " + auditPreserved);
    bool passed = validAccepted && malformedRejected && auditPreserved;
    Console.WriteLine(passed ? "VERIFY PASS" : "VERIFY FAIL");
    return passed ? 0 : 1;
}

if (mode != "--demo")
{
    Console.Error.WriteLine("Use --demo, --reproduce or --verify.");
    return 2;
}

return 0;

record DispatchTicket(string TicketId, int Priority);
record AuditEntry(string Route, string Sha256, int CharacterCount);
record ProbeResult(int ValidStatus, string ValidBody, int MalformedStatus, IReadOnlyList<AuditEntry> AuditEntries);

sealed class RequestAuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICollection<AuditEntry> _audit;

    public RequestAuditMiddleware(RequestDelegate next, ICollection<AuditEntry> audit)
    {
        _next = next;
        _audit = audit;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using var reader = new StreamReader(
            context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false,
            bufferSize: 1024, leaveOpen: true);
        var payload = await reader.ReadToEndAsync();
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload)));
        _audit.Add(new AuditEntry(context.Request.Path.ToString(), hash, payload.Length));
        await _next(context);
    }
}

static class DispatchProbe
{
    public static async Task<ProbeResult> RunAsync()
    {
        var audit = new List<AuditEntry>();
        var valid = await SendAsync("{\"TicketId\":\"V-104\",\"Priority\":2}", audit);
        var malformed = await SendAsync("{\"TicketId\":", audit);
        return new ProbeResult(valid.Status, valid.Body, malformed.Status, audit);
    }

    private static async Task<(int Status, string Body)> SendAsync(string json, ICollection<AuditEntry> audit)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/dispatch";
        context.Request.ContentType = "application/json";
        context.Request.Body = new ForwardOnlyStream(new MemoryStream(Encoding.UTF8.GetBytes(json)));
        context.Response.Body = new MemoryStream();

        RequestDelegate endpoint = async ctx =>
        {
            try
            {
                var ticket = await JsonSerializer.DeserializeAsync<DispatchTicket>(ctx.Request.Body);
                if (ticket is null || string.IsNullOrWhiteSpace(ticket.TicketId) || ticket.Priority < 1)
                {
                    ctx.Response.StatusCode = 400;
                    return;
                }

                ctx.Response.StatusCode = 200;
                await ctx.Response.WriteAsync("accepted:" + ticket.TicketId);
            }
            catch (JsonException)
            {
                ctx.Response.StatusCode = 400;
            }
        };

        var middleware = new RequestAuditMiddleware(endpoint, audit);
        await middleware.InvokeAsync(context);
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        var status = context.Response.StatusCode;
        await context.Request.Body.DisposeAsync();
        await context.Response.Body.DisposeAsync();
        return (status, body);
    }
}

// Models a network request stream: forward-only, not an in-memory seekable shortcut.
sealed class ForwardOnlyStream : Stream
{
    private readonly Stream _inner;
    public ForwardOnlyStream(Stream inner) => _inner = inner;
    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        => _inner.ReadAsync(buffer, cancellationToken);
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => _inner.ReadAsync(buffer, offset, count, cancellationToken);
    public override void Flush() { }
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    protected override void Dispose(bool disposing) { if (disposing) _inner.Dispose(); base.Dispose(disposing); }
    public override ValueTask DisposeAsync() => _inner.DisposeAsync();
}
