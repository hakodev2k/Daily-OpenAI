using System.Text;

var pool = new ReusableBuffer(64);
var builder = new PayloadBuilder(pool);

var first = builder.Build("ORDER-1001");
var firstImmediately = Encoding.UTF8.GetString(first.Span);
Console.WriteLine($"First immediately: {firstImmediately}");

var second = builder.Build("ORDER-9999");
var firstAfterSecondBuild = Encoding.UTF8.GetString(first.Span);
Console.WriteLine($"Second: {Encoding.UTF8.GetString(second.Span)}");
Console.WriteLine($"First afterwards: {firstAfterSecondBuild}");

if (firstImmediately != "ORDER-1001") return 2;
if (firstAfterSecondBuild != "ORDER-1001")
{
    Console.Error.WriteLine("FAIL: caller-visible payload changed after a later operation.");
    return 1;
}

Console.WriteLine("PASS: previously returned payload stayed stable.");
return 0;

sealed class PayloadBuilder(ReusableBuffer pool)
{
    public ReadOnlyMemory<byte> Build(string value)
    {
        var buffer = pool.Rent();
        var written = Encoding.UTF8.GetBytes(value, buffer);
        var result = new ReadOnlyMemory<byte>(buffer, 0, written);
        pool.Return(buffer);
        return result;
    }
}

sealed class ReusableBuffer(int size)
{
    private readonly byte[] _buffer = new byte[size];
    public byte[] Rent() => _buffer;
    public void Return(byte[] buffer)
    {
        if (!ReferenceEquals(buffer, _buffer)) throw new InvalidOperationException();
    }
}
