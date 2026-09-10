using System.Buffers;
using System.Text;

var pool = ArrayPool<byte>.Create(maxArrayLength: 64, maxArraysPerBucket: 1);

var tenantA = Encoding.UTF8.GetBytes("tenant-a|secret=ALPHA-42");
var bufferA = pool.Rent(32);
tenantA.CopyTo(bufferA, 0);
Console.WriteLine($"A_WRITTEN={Encoding.UTF8.GetString(bufferA, 0, tenantA.Length)}");

pool.Return(bufferA, clearArray: true);

var tenantB = Encoding.UTF8.GetBytes("tenant-b|payload=BETA");
var bufferB = pool.Rent(32);
var snapshot = Encoding.UTF8.GetString(bufferB, 0, Math.Min(tenantA.Length, bufferB.Length));
Console.WriteLine($"B_INITIAL_SNAPSHOT={snapshot}");

Array.Clear(bufferB, 0, bufferB.Length);
tenantB.CopyTo(bufferB, 0);
var processedB = Encoding.UTF8.GetString(bufferB, 0, tenantB.Length);
Console.WriteLine($"B_PROCESSED={processedB}");
pool.Return(bufferB, clearArray: true);

var leaked = snapshot.Contains("ALPHA-42", StringComparison.Ordinal);
var functional = processedB == "tenant-b|payload=BETA";
Console.WriteLine(!leaked && functional ? "REFERENCE_PASS" : "REFERENCE_FAIL");
return !leaked && functional ? 0 : 1;
