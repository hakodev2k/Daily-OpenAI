using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

var builder = WebApplication.CreateBuilder();
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Logging.ClearProviders();
var app = builder.Build();
var partner = new PartnerState();

app.MapPost("/legacy/evidence", () => Results.Redirect("/api/evidence"));
app.MapPost("/legacy/decoy", () => Results.Redirect("/api/status"));
app.MapGet("/api/evidence", () => Results.Content("<html>Partner upload portal</html>", "text/html"));
app.MapGet("/api/status", () => Results.Content("<html>Partner status: OK</html>", "text/html"));
app.MapPost("/api/evidence", (Evidence evidence) =>
{
    partner.ReceivedIds.Add(evidence.Id);
    return Results.Json(new Receipt("RCPT-" + evidence.Id, true));
});

await app.StartAsync();
try
{
    var server = app.Services.GetRequiredService<IServer>();
    var feature = server.Features.Get<IServerAddressesFeature>()
        ?? throw new InvalidOperationException("Loopback address unavailable.");
    var address = feature.Addresses.Single(a => a.StartsWith("http://127.0.0.1:", StringComparison.Ordinal));
    using var gateway = new PartnerGateway(new Uri(address));

    var first = await gateway.SubmitAsync("/legacy/evidence", new Evidence("EV-217"), CancellationToken.None);
    var second = await gateway.SubmitAsync("/legacy/decoy", new Evidence("EV-218"), CancellationToken.None);

    Console.WriteLine($"legacy/evidence: accepted={first.Accepted}, receipt={first.ReceiptId ?? "<none>"}, status={first.HttpStatus}");
    Console.WriteLine($"legacy/decoy: accepted={second.Accepted}, receipt={second.ReceiptId ?? "<none>"}, status={second.HttpStatus}");
    Console.WriteLine($"partner durable receipts: {string.Join(",", partner.ReceivedIds)}");

    var contractOk = first.Accepted
        && first.ReceiptId == "RCPT-EV-217"
        && !second.Accepted
        && second.ReceiptId is null
        && partner.ReceivedIds.SequenceEqual(new[] { "EV-217" });

    Console.WriteLine(contractOk ? "CONTRACT_OK" : "OBSERVED_FAILURE");
    return (args.FirstOrDefault() ?? "inspect") switch
    {
        "reproduce" => contractOk ? 2 : 0,
        "verify" => contractOk ? 0 : 3,
        _ => 0
    };
}
finally
{
    await app.StopAsync();
}

public sealed record Evidence(string Id);
public sealed record Receipt(string Id, bool Accepted);
public sealed record SubmissionResult(bool Accepted, string? ReceiptId, int HttpStatus);
public sealed class PartnerState
{
    public List<string> ReceivedIds { get; } = new();
}
