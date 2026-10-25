namespace PermitLab;

public sealed class PermitApprovalService
{
    private readonly IPermitStore _store;

    public PermitApprovalService(IPermitStore store) => _store = store;

    public async Task<bool> ApproveAsync(string permitId)
    {
        var permit = await _store.FindAsync(permitId);
        if (permit is null || permit.Status != "Pending")
            return false;

        permit.Status = "Approved";
        permit.Version++;
        await _store.SaveAsync(permit);
        return true;
    }
}
