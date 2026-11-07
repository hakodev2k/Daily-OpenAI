using System.Globalization;
using System.Xml.Linq;

namespace CertificateLab;

public sealed record BatchSummary(string BatchId, int ReadingCount, decimal TotalKwh);

public static class CertificateImporter
{
    private static readonly XNamespace ContractNamespace = "urn:energy:certificate:v2";

    public static BatchSummary Import(string xml)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xml);

        var document = XDocument.Parse(xml, LoadOptions.None);
        var root = document.Root ?? throw new InvalidDataException("Missing batch root.");

        if (root.Name != ContractNamespace + "Settlement")
            throw new InvalidDataException("Unsupported settlement XML contract.");

        var batchId = (string?)root.Attribute("batch");
        if (string.IsNullOrWhiteSpace(batchId))
            throw new InvalidDataException("Missing batch ID.");

        var count = 0;
        var total = 0m;

        foreach (var meter in root.Elements(ContractNamespace + "Meter"))
        {
            foreach (var reading in meter.Elements(ContractNamespace + "Reading"))
            {
                var raw = (string?)reading.Attribute("kwh");
                if (string.IsNullOrWhiteSpace(raw))
                    throw new InvalidDataException("Missing kwh attribute.");

                if (!decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var kwh))
                    throw new InvalidDataException("Invalid kwh value.");

                total += kwh;
                count++;
            }
        }

        return new BatchSummary(batchId, count, total);
    }
}
