using System.Buffers;
using System.Text;

var mode = args.Contains("--verify", StringComparer.OrdinalIgnoreCase) ? "verify" : "reproduce";
var pool = ArrayPool<byte>.Create(maxArrayLength: 64, maxArraysPerBucket: 1);

var tenantA = Encoding.UTF8.GetBytes("tenant-a|secret=ALPHA-42");
var bufferA = pool.Rent(32);
tenantA.CopyTo(bufferA, 0);
Console.WriteLine($"A_WRITTEN={Encoding.UTF8.GetString(bufferA, 0, tenantA.Length)}");

// Investigation note:
// This worker is optimized to reuse temporary buffers between requests.
pool.Return(bufferA);

var tenantB = Encoding.UTF8.GetBytes("tenant-b|payload=BETA");
var bufferB = pool.Rent(32);
var snapshot = Encoding.UTF8.GetString(bufferB, 0, Math.Min(tenantA.Length, bufferB.Length));
Console.WriteLine($"B_INITIAL_SNAPSHOT={snapshot}");

Array.Clear(bufferB, 0, bufferB.Length);
tenantB.CopyTo(bufferB, 0);
var processedB = Encoding.UTF8.GetString(bufferB, 0, tenantB.Length);
Console.WriteLine($"B_PROCESSED={processedB}");
pool.Return(bufferB);

var leaked = snapshot.Contains("ALPHA-42", StringComparison.Ordinal);
var functional = processedB == "tenant-b|payload=BETA";

if (mode == "reproduce")
{
    Console.WriteLine(leaked ? "LAB_REPRODUCED" : "LAB_NOT_REPRODUCED");
    return leaked && functional ? 0 : 1;
}

Console.WriteLine(!leaked && functional ? "LAB_VERIFY_PASS" : "LAB_VERIFY_FAIL");
return !leaked && functional ? 0 : 1;
