using System.Buffers;

var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var source = new byte[] { 10, 20, 30, 40 };
var pipeline = new ThumbnailPipeline();
var pending = pipeline.SubmitAsync(source, gate.Task);
Console.WriteLine("queued thumbnail; waiting for storage gate");
var competing = DeterministicPool.Shared.Rent(source.Length);
try
{
    Array.Fill(competing, (byte)99, 0, source.Length);
    Console.WriteLine("competing workload wrote temporary bytes");
    gate.SetResult();
    var actual = await pending;
    Console.WriteLine($"expected={Convert.ToHexString(source)} actual={Convert.ToHexString(actual)}");
    if (!actual.SequenceEqual(source))
    {
        Console.Error.WriteLine("FAIL: thumbnail integrity mismatch");
        Environment.ExitCode = 1;
    }
    else Console.WriteLine("PASS: thumbnail integrity preserved");
}
finally { DeterministicPool.Shared.Return(competing, clearArray: true); }

sealed class ThumbnailPipeline
{
    public async Task<byte[]> SubmitAsync(byte[] thumbnail, Task storageGate)
    {
        var rented = DeterministicPool.Shared.Rent(thumbnail.Length);
        try
        {
            thumbnail.CopyTo(rented, 0);
            return await StoreLaterAsync(rented, thumbnail.Length, storageGate);
        }
        finally
        {
            DeterministicPool.Shared.Return(rented);
        }
    }

    private static async Task<byte[]> StoreLaterAsync(byte[] bytes, int length, Task storageGate)
    {
        await storageGate;
        return bytes.AsSpan(0, length).ToArray();
    }
}

sealed class DeterministicPool
{
    public static readonly DeterministicPool Shared = new();
    private byte[]? _available;
    public byte[] Rent(int minimumLength)
    {
        var item = _available;
        if (item is not null && item.Length >= minimumLength) { _available = null; return item; }
        return new byte[minimumLength];
    }
    public void Return(byte[] item, bool clearArray = false)
    {
        if (clearArray) Array.Clear(item);
        _available = item;
    }
}
