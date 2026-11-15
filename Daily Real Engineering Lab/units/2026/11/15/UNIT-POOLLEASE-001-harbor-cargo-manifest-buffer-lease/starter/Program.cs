using System.Buffers;
using System.Text;
var pool = new ReusePool();
var svc = new ManifestService(pool);
var a = svc.Read("SHIP-A|CONTAINERS=12");
Console.WriteLine("Before: " + Encoding.UTF8.GetString(a.Span));
var b = svc.Read("SHIP-B|CONTAINERS=99");
Console.WriteLine("After: " + Encoding.UTF8.GetString(a.Span));
Console.WriteLine("Other: " + Encoding.UTF8.GetString(b.Span));
if (Encoding.UTF8.GetString(a.Span) != "SHIP-A|CONTAINERS=12") throw new Exception("FAIL: historical manifest was mutated");
Console.WriteLine("PASS");
public sealed class ManifestService
{
    readonly ArrayPool<byte> pool;
    public ManifestService(ArrayPool<byte> pool) => this.pool = pool;
    public ReadOnlyMemory<byte> Read(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        var rented = pool.Rent(bytes.Length);
        try { bytes.CopyTo(rented, 0); return rented.AsMemory(0, bytes.Length); }
        finally { pool.Return(rented); }
    }
}
public sealed class ReusePool : ArrayPool<byte>
{
    byte[]? buffer;
    public override byte[] Rent(int n) => buffer = buffer is null || buffer.Length < n ? new byte[Math.Max(64,n)] : buffer;
    public override void Return(byte[] b, bool clearArray = false) { if(clearArray) Array.Clear(b); buffer=b; }
}
