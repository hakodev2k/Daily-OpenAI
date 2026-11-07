using System.Security.Cryptography;
using System.Text;

var payload = Encoding.UTF8.GetBytes("contract-2026-09-09|customer=42|version=7");
await using var input = new MemoryStream(payload, writable: false);

using var sha = SHA256.Create();
var expectedHash = Convert.ToHexString(await sha.ComputeHashAsync(input));

input.Position = 0;

await using var destination = new MemoryStream();
await input.CopyToAsync(destination);
var stored = destination.ToArray();
var storedHash = Convert.ToHexString(SHA256.HashData(stored));

Console.WriteLine($"Input bytes={payload.Length}");
Console.WriteLine($"Stored bytes={stored.Length}");
Console.WriteLine($"Content matches={payload.SequenceEqual(stored)}");
Console.WriteLine($"Hash matches={expectedHash == storedHash}");
