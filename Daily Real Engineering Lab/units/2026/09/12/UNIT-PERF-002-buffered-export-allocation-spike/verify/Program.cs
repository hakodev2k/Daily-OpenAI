const int payloadBytes = 8 * 1024 * 1024;
var payload = new byte[payloadBytes];
for (var i = 0; i < payload.Length; i++)
{
    payload[i] = (byte)(i % 251);
}

using var functionalDestination = new MemoryStream(capacity: payload.Length);
ReportExporter.WriteExport(functionalDestination, payload);
var output = functionalDestination.ToArray();
var functionalOk = output.AsSpan().SequenceEqual(payload);

GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

using var allocationDestination = Stream.Null;
var before = GC.GetAllocatedBytesForCurrentThread();
ReportExporter.WriteExport(allocationDestination, payload);
var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

Console.WriteLine($"FUNCTIONAL_OK={functionalOk}");
Console.WriteLine($"PAYLOAD_BYTES={payload.Length}");
Console.WriteLine($"ALLOCATED_BYTES={allocated}");
Console.WriteLine($"ALLOCATION_RATIO={(double)allocated / payload.Length:F4}");

if (!functionalOk)
{
    Console.Error.WriteLine("Verification failed: exported bytes changed.");
    return 2;
}

if (allocated > 1_048_576)
{
    Console.Error.WriteLine("Verification failed: export path still allocates more than 1 MiB for an 8 MiB payload.");
    return 3;
}

Console.WriteLine("VERIFY_OK");
return 0;
