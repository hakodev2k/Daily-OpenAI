namespace SealImportLab;

public sealed record SealScan(string SealCode, string Gate, bool PassedInspection);

public sealed record ImportReport(int Received, int Stored, int Rejected);

public sealed class ScanImportService
{
    public ImportReport Import(IEnumerable<SealScan> scans, ICollection<SealScan> destination)
    {
        ArgumentNullException.ThrowIfNull(scans);
        ArgumentNullException.ThrowIfNull(destination);

        var received = 0;
        var stored = 0;
        var rejected = 0;

        foreach (var scan in scans)
        {
            received++;
            if (!string.IsNullOrWhiteSpace(scan.SealCode)
                && !string.IsNullOrWhiteSpace(scan.Gate)
                && scan.PassedInspection)
            {
                destination.Add(scan);
                stored++;
            }
            else
            {
                rejected++;
            }
        }

        return new ImportReport(received, stored, rejected);
    }
}
