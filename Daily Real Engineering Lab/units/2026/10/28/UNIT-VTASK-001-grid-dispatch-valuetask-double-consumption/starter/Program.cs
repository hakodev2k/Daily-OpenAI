using GridSignalLab;

return args.Length == 1 && args[0] == "--verify"
    ? await VerifyAsync()
    : args.Length == 1 && args[0] == "--reproduce"
        ? await ReproduceAsync()
        : Usage();

static int Usage()
{
    Console.Error.WriteLine("Use --reproduce or --verify");
    return 2;
}

static async Task<int> ReproduceAsync()
{
    var client = new SimulatedSignalClient();
    var audit = new AuditTrail();
    var dispatch = new TelemetryDispatch(client, audit);
    try
    {
        await dispatch.HandleAsync("GRID-NORTH-11");
        Console.Error.WriteLine("BASELINE_UNEXPECTED_SUCCESS");
        return 1;
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("already consumed", StringComparison.Ordinal))
    {
        Console.WriteLine("GATEWAY_REQUESTS=" + client.RequestCount);
        Console.WriteLine("AUDIT_EVENTS=" + audit.Events.Count);
        Console.WriteLine("OBSERVED_EXCEPTION=" + ex.GetType().Name);
        Console.WriteLine("REPRODUCED: valid signal failed before receipt");
        return client.RequestCount == 1 && audit.Events.Count == 0 ? 0 : 1;
    }
}

static async Task<int> VerifyAsync()
{
    var client = new SimulatedSignalClient();
    var audit = new AuditTrail();
    var dispatch = new TelemetryDispatch(client, audit);
    try
    {
        var north = await dispatch.HandleAsync("GRID-NORTH-11");
        var south = await dispatch.HandleAsync("GRID-SOUTH-12");

        bool receipts = north == new DispatchReceipt("GRID-NORTH-11", 42)
                     && south == new DispatchReceipt("GRID-SOUTH-12", 67);
        bool auditCorrect = audit.Events.Count == 2
                         && audit.Events[0] == new AuditEvent("GRID-NORTH-11", 42)
                         && audit.Events[1] == new AuditEvent("GRID-SOUTH-12", 67);
        bool oneGatewayCallPerSignal = client.RequestCount == 2;
        Console.WriteLine("RECEIPTS_OK=" + receipts);
        Console.WriteLine("AUDIT_OK=" + auditCorrect);
        Console.WriteLine("GATEWAY_REQUESTS=" + client.RequestCount);
        if (!receipts || !auditCorrect || !oneGatewayCallPerSignal) return 1;
        Console.WriteLine("VERIFIED: learner dispatch satisfies all contracts");
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("VERIFY_FAILED=" + ex.GetType().Name + ": " + ex.Message);
        return 1;
    }
}

namespace GridSignalLab
{
    public sealed record DispatchReceipt(string SignalId, int ValidatedKilowatts);
    public sealed record AuditEvent(string SignalId, int ValidatedKilowatts);

    public sealed class AuditTrail
    {
        private readonly List<AuditEvent> _events = new();
        public IReadOnlyList<AuditEvent> Events => _events;
        public void Record(string signalId, int validatedKilowatts)
            => _events.Add(new AuditEvent(signalId, validatedKilowatts));
    }
}
