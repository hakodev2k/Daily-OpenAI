using System.Text;
using System.Security.Cryptography;

var payload = Encoding.UTF8.GetBytes("REPORT:7");
var buffer = new MemoryStream(capacity: 128);
buffer.Write(payload);
byte[] exported = buffer.ToArray();
var hash = Convert.ToHexString(SHA256.HashData(exported));
Console.WriteLine($"LogicalLength={buffer.Length}; ExportedLength={exported.Length}; SHA256={hash}");
Console.WriteLine($"ExportedText={Encoding.UTF8.GetString(exported)}");
bool valid = exported.Length == payload.Length && exported.AsSpan().SequenceEqual(payload);
if (args.Contains("--check"))
{
    Console.WriteLine(valid ? "PASS: exact payload" : "FAIL: exported bytes differ from logical payload");
    return valid ? 0 : 2;
}
return 0;
