using System.Text;

var directory = Path.Combine(Path.GetTempPath(), "wildlife-census-lab-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(directory);
var target = Path.Combine(directory, "census.csv");
const string oldReport = "species,count\nOtter,12\nHeron,7\n";
const string newReport = "species,count\nOtter,13\nHeron,8\n";
try
{
    File.WriteAllText(target, oldReport, new UTF8Encoding(false));
    try
    {
        CensusWriter.Replace(target, newReport, failAfterBytes: 10);
        Console.WriteLine("UNEXPECTED: injected fault did not occur");
        return 2;
    }
    catch (IOException ex) when (ex.Message == "Injected write failure")
    {
        Console.WriteLine("INJECTED_FAULT=observed");
    }

    string observed = File.ReadAllText(target, Encoding.UTF8);
    bool preserved = observed == oldReport;
    Console.WriteLine("OLD_REPORT_PRESERVED=" + preserved.ToString().ToLowerInvariant());
    Console.WriteLine("OBSERVED_BYTES=" + new FileInfo(target).Length);
    if (!preserved) Console.WriteLine("EVIDENCE=previously committed census changed after failed refresh");

    CensusWriter.Replace(target, newReport, failAfterBytes: null);
    bool newCommitted = File.ReadAllText(target, Encoding.UTF8) == newReport;
    bool noOrphans = Directory.GetFiles(directory).Length == 1;
    Console.WriteLine("NEW_REPORT_COMMITTED=" + newCommitted.ToString().ToLowerInvariant());
    Console.WriteLine("NO_ORPHAN_TEMP_FILES=" + noOrphans.ToString().ToLowerInvariant());
    bool passed = preserved && newCommitted && noOrphans;
    Console.WriteLine("CONTRACT=" + (passed ? "PASS" : "FAIL"));
    return passed ? 0 : 1;
}
finally
{
    Directory.Delete(directory, recursive: true);
}

static class CensusWriter
{
    public static void Replace(string target, string report, int? failAfterBytes)
    {
        byte[] bytes = new UTF8Encoding(false).GetBytes(report);
        using var stream = new FileStream(target, FileMode.Create, FileAccess.Write, FileShare.None);
        int first = failAfterBytes.HasValue ? Math.Min(failAfterBytes.Value, bytes.Length) : bytes.Length;
        stream.Write(bytes, 0, first);
        stream.Flush(flushToDisk: true);
        if (failAfterBytes.HasValue) throw new IOException("Injected write failure");
        if (first < bytes.Length) stream.Write(bytes, first, bytes.Length - first);
        stream.Flush(flushToDisk: true);
    }
}
