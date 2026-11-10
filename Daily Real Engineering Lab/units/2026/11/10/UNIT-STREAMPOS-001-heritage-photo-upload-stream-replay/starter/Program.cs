using System.Security.Cryptography;
using System.Text;
var bytes = Encoding.UTF8.GetBytes("Museum photo batch - archival sample 2026");
using var source = new MemoryStream(bytes);
var receipt = await new ArchiveUploader().UploadAsync(source);
Console.WriteLine($"SOURCE_BYTES={bytes.Length}");
Console.WriteLine($"STORED_BYTES={receipt.StoredBytes.Length}");
Console.WriteLine($"SHA256={receipt.Sha256}");
Console.WriteLine($"SOURCE_POSITION={receipt.SourcePosition}");
if (receipt.Sha256 != Convert.ToHexString(SHA256.HashData(bytes)) || !receipt.StoredBytes.SequenceEqual(bytes))
{ Console.Error.WriteLine("REPRODUCED: object differs from source"); return 2; }
Console.WriteLine("PASS"); return 0;
