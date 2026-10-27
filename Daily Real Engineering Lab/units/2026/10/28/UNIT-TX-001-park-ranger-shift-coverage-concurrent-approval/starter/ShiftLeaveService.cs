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
        var roster = _store.ReadSnapshot();
        await _gate.ArriveAsync(); // Mô phỏng hai request tiến đến cùng một giai đoạn xử lý.
        if (roster.ActiveCount <= 1) return false;
        return _store.MarkOffDuty(staffId);
    }
}
