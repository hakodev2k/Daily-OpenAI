namespace GridSignalLab;

public sealed class TelemetryDispatch
{
    private readonly SimulatedSignalClient _client;
    private readonly AuditTrail _audit;

    public TelemetryDispatch(SimulatedSignalClient client, AuditTrail audit)
    {
        _client = client;
        _audit = audit;
    }

    public async Task<DispatchReceipt> HandleAsync(string signalId)
    {
        var pending = _client.ReadValidatedKilowattsAsync(signalId);

        var validatedKilowatts = await pending;
        _audit.Record(signalId, await pending);

        return new DispatchReceipt(signalId, validatedKilowatts);
    }
}
