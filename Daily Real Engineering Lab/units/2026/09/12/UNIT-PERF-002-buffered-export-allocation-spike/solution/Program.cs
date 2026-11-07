const int payloadBytes = 8 * 1024 * 1024;
var payload = new byte[payloadBytes];
for (var i = 0; i < payload.Length; i += 4096)
{
    payload[i] = (byte)(i % 251);
}

using var destination = Stream.Null;
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

var before = GC.GetAllocatedBytesForCurrentThread();
ReportExporter.WriteExport(destination, payload);
var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

Console.WriteLine($"PAYLOAD_BYTES={payload.Length}");
Console.WriteLine($"ALLOCATED_BYTES={allocated}");
Console.WriteLine($"ALLOCATION_RATIO={(double)allocated / payload.Length:F2}");

public static class ReportExporter
{
    public static void WriteExport(Stream destination, ReadOnlySpan<byte> payload)
    {
        destination.Write(payload);
    }
}
