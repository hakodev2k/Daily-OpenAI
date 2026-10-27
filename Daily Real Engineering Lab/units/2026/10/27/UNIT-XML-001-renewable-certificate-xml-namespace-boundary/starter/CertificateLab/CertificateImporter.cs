using System.Globalization;
using System.Xml.Linq;

namespace CertificateLab;

public sealed record BatchSummary(string BatchId, int ReadingCount, decimal TotalKwh);

public static class CertificateImporter
{
    public static BatchSummary Import(string xml)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xml);

        var document = XDocument.Parse(xml, LoadOptions.None);
        var root = document.Root ?? throw new InvalidDataException("Missing batch root.");
        var batchId = (string?)root.Attribute("batch")
            ?? throw new InvalidDataException("Missing batch ID.");

        // Investigation: does the selected data represent the contractual meter readings?
        var readings = document.Descendants("Reading");

        var count = 0;
        var total = 0m;
        foreach (var reading in readings)
        {
            var raw = (string?)reading.Attribute("kwh")
                ?? throw new InvalidDataException("Missing kwh attribute.");
            total += decimal.Parse(raw, CultureInfo.InvariantCulture);
            count++;
        }

        return new BatchSummary(batchId, count, total);
    }
}
