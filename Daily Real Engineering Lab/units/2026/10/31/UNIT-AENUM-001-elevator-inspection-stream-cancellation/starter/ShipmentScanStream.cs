namespace ElevatorScanLab;

public sealed record ScanReceipt(int Sequence, string GateId);

public sealed class ShipmentScanStream
{
    // API được dùng bởi nhiều màn hình kiểm định.
    public async IAsyncEnumerable<ScanReceipt> ReadAsync(
        CancellationToken cancellationToken = default)
    {
        for (var sequence = 1; sequence <= 4; sequence++)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(25), cancellationToken);
            yield return new ScanReceipt(sequence, $"LIFT-{sequence:000}");
        }
    }
}
