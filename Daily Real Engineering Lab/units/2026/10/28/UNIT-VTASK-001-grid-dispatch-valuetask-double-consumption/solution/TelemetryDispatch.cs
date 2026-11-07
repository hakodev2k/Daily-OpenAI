namespace GridSignalLab;

// Reference Solution — chỉ xem sau khi tự reproduce và thử sửa.
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
        int validatedKilowatts = await _client.ReadValidatedKilowattsAsync(signalId);
        _audit.Record(signalId, validatedKilowatts);
        return new DispatchReceipt(signalId, validatedKilowatts);
    }
}
