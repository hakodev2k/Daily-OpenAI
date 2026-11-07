using System.IO.Compression;
using System.Text;

internal static class Program
{
    private static int Main(string[] args)
    {
        var mode = args.Length == 1 ? args[0] : "--diagnose";
        if (mode is not ("--diagnose" or "--reproduce" or "--verify"))
        {
            Console.Error.WriteLine("Usage: --diagnose | --reproduce | --verify");
            return 2;
        }

        var service = new EvidenceExportService();
        var documents = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["manifest.json"] = "{\"batch\":\"CV-2026-41\",\"count\":1}",
            ["reports/site-inspection.txt"] = "Inspection CV-2026-41: signed and archived."
        };
        var results = new List<ProbeResult>();
        for (var attempt = 1; attempt <= 3; attempt++)
        {
            var payload = service.CreatePackage(documents);
            var result = Probe(payload, documents);
            results.Add(result);
            Console.WriteLine($"attempt={attempt} bytes={payload.Length} status={result.Status} detail={result.Detail}");
        }

        var empty = service.CreatePackage(new Dictionary<string, string>(StringComparer.Ordinal));
        var emptyResult = Probe(empty, new Dictionary<string, string>(StringComparer.Ordinal));
        Console.WriteLine($"empty-package bytes={empty.Length} status={emptyResult.Status}");

        if (mode == "--diagnose")
            return 0;

        if (mode == "--reproduce")
        {
            var reproduced = results.All(x => x.Status == ProbeStatus.InvalidContainer)
                && emptyResult.Status == ProbeStatus.InvalidContainer;
            Console.WriteLine(reproduced ? "REPRODUCED: invalid ZIP containers" : "REPRODUCTION FAILED: unexpected symptom");
            return reproduced ? 0 : 1;
        }

        var verified = results.All(x => x.Status == ProbeStatus.Valid)
            && emptyResult.Status == ProbeStatus.Valid;
        Console.WriteLine(verified ? "VERIFIED: ZIP integrity and content contract" : "VERIFICATION FAILED: ZIP integrity or content contract");
        return verified ? 0 : 1;
    }

    private static ProbeResult Probe(byte[] bytes, IReadOnlyDictionary<string, string> expected)
    {
        try
        {
            using var input = new MemoryStream(bytes);
            using var zip = new ZipArchive(input, ZipArchiveMode.Read);
            if (zip.Entries.Count != expected.Count)
                return new(ProbeStatus.WrongContent, $"entry-count={zip.Entries.Count}; expected={expected.Count}");

            foreach (var item in expected)
            {
                var entry = zip.GetEntry(item.Key);
                if (entry is null)
                    return new(ProbeStatus.WrongContent, $"missing-entry={item.Key}");

                using var reader = new StreamReader(entry.Open(), Encoding.UTF8);
                var actual = reader.ReadToEnd();
                if (!string.Equals(actual, item.Value, StringComparison.Ordinal))
                    return new(ProbeStatus.WrongContent, $"content-mismatch={item.Key}");
            }
            return new(ProbeStatus.Valid, "entries and content match");
        }
        catch (InvalidDataException ex)
        {
            return new(ProbeStatus.InvalidContainer, ex.GetType().Name);
        }
    }
}

internal enum ProbeStatus { Valid, InvalidContainer, WrongContent }
internal readonly record struct ProbeResult(ProbeStatus Status, string Detail);

internal sealed class EvidenceExportService
{
    public byte[] CreatePackage(IReadOnlyDictionary<string, string> documents)
    {
        using var buffer = new MemoryStream();
        using var archive = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true);
        foreach (var item in documents.OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            var entry = archive.CreateEntry(item.Key, CompressionLevel.Optimal);
            using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
            writer.Write(item.Value);
        }

        return buffer.ToArray();
    }
}
