using System.Buffers;
using System.Text;

var pool = new ReusablePool();
var encoder = new DispatchEncoder(pool);
var first = encoder.Encode("ALPHA");
var second = encoder.Encode("BRAVO");
Console.WriteLine($"First dispatch: {Encoding.ASCII.GetString(first.Span)}");
Console.WriteLine($"Second dispatch: {Encoding.ASCII.GetString(second.Span)}");
if (Encoding.ASCII.GetString(first.Span) != "ALPHA" ||
    Encoding.ASCII.GetString(second.Span) != "BRAVO")
{
    Console.Error.WriteLine("FAIL: payload snapshot changed after subsequent dispatch");
    Environment.ExitCode = 1;
}
else Console.WriteLine("PASS: independent immutable dispatch payloads");

sealed class DispatchEncoder
{
    private readonly ArrayPool<byte> _pool;
    public DispatchEncoder(ArrayPool<byte> pool) => _pool = pool;
    public ReadOnlyMemory<byte> Encode(string code)
    {
        if (code.Length != 5) throw new ArgumentException("Five-character dispatch code required");
        byte[] buffer = _pool.Rent(5);
        try
        {
            Encoding.ASCII.GetBytes(code.AsSpan(), buffer.AsSpan(0, 5));
            return buffer.AsSpan(0, 5).ToArray();
        }
        finally { _pool.Return(buffer); }
    }
}

// Deterministic stand-in for a pool that immediately reuses returned storage.
sealed class ReusablePool : ArrayPool<byte>
{
    private byte[]? _available;
    public override byte[] Rent(int minimumLength)
    {
        var buffer = _available;
        if (buffer is not null && buffer.Length >= minimumLength)
        {
            _available = null;
            return buffer;
        }
        return new byte[Math.Max(5, minimumLength)];
    }
    public override void Return(byte[] array, bool clearArray = false)
    {
        if (clearArray) Array.Clear(array);
        _available = array;
    }
}
