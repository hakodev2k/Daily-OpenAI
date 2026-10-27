public sealed class ShiftLeaveService
{
    private readonly ShiftRosterStore _store;
    private readonly TwoPartyGate _gate;

    public ShiftLeaveService(ShiftRosterStore store, TwoPartyGate gate)
    {
        _store = store;
        _gate = gate;
    }

    public async Task<bool> ApproveAsync(string staffId)
    {
        await _gate.ArriveAsync();
        return _store.TryApproveLeaveKeepingCoverage(staffId, minimumOnDuty: 1);
    }
}
