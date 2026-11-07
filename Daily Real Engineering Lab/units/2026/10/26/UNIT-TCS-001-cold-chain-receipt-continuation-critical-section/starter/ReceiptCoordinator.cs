namespace ColdChainReceiptLab;

public sealed record PalletReceipt(string PalletId, string Result);

/// <summary>
/// Matches incoming scanner receipts to in-flight pallet requests.
/// </summary>
public sealed class ReceiptCoordinator
{
    private readonly object _gate = new();
    private readonly Dictionary<string, TaskCompletionSource<PalletReceipt>> _pending =
        new(StringComparer.Ordinal);

    public void Track(string palletId)
    {
        lock (_gate)
        {
            _pending.Add(palletId, new TaskCompletionSource<PalletReceipt>());
        }
    }

    public async Task<PalletReceipt> WaitAndAuditAsync(
        string palletId, Action<PalletReceipt> audit)
    {
        Task<PalletReceipt> pending;
        lock (_gate)
        {
            pending = _pending[palletId].Task;
        }

        var receipt = await pending.ConfigureAwait(false);
        audit(receipt);
        return receipt;
    }

    public bool Accept(PalletReceipt receipt)
    {
        lock (_gate)
        {
            if (!_pending.Remove(receipt.PalletId, out var pending))
                return false;

            return pending.TrySetResult(receipt);
        }
    }

    public bool IsGateEnteredByCurrentThread => Monitor.IsEntered(_gate);
}
