using CertificateLab;

var mode = args.Length == 0 ? "inspect" : args[0];

try
{
    return mode switch
    {
        "inspect" => Inspect(),
        "reproduce" => Reproduce(),
        "verify" => Verify(),
        _ => Usage()
    };
}
catch (Exception ex)
{
    Console.Error.WriteLine($"UNEXPECTED {ex.GetType().Name}: {ex.Message}");
    return 2;
}

static int Usage()
{
    Console.Error.WriteLine("Usage: dotnet run -- [inspect|reproduce|verify]");
    return 2;
}

static int Inspect()
{
    var summary = CertificateImporter.Import(Fixtures.DefaultNamespace);
    Console.WriteLine($"Batch={summary.BatchId}; readingCount={summary.ReadingCount}; totalKwh={summary.TotalKwh}");
    Console.WriteLine("Partner reconciliation: expected two contractual readings totaling 20 kWh.");
    return 0;
}

static int Reproduce()
{
    var summary = CertificateImporter.Import(Fixtures.DefaultNamespace);
    Console.WriteLine($"OBSERVED batch={summary.BatchId}, readings={summary.ReadingCount}, totalKwh={summary.TotalKwh}");
    var reproduced = summary.BatchId == "OCT-27" && summary.ReadingCount == 0 && summary.TotalKwh == 0m;
    Console.WriteLine(reproduced ? "REPRODUCED: accepted batch with zero readings." : "NOT REPRODUCED: inspect current starter.");
    return reproduced ? 0 : 1;
}

static int Verify()
{
    var failures = 0;
    CheckSummary("default-namespace payload", Fixtures.DefaultNamespace);
    CheckSummary("prefix-equivalent payload", Fixtures.PrefixedEquivalent);
    CheckReject("unsupported contract version", Fixtures.UnsupportedVersion);
    CheckReject("missing kwh attribute", Fixtures.MissingEnergyValue);
    Console.WriteLine(failures == 0 ? "VERIFY PASS" : $"VERIFY FAIL: {failures} checks failed.");
    return failures == 0 ? 0 : 1;

    void CheckSummary(string label, string xml)
    {
        try
        {
            var actual = CertificateImporter.Import(xml);
            var ok = actual.BatchId == "OCT-27" && actual.ReadingCount == 2 && actual.TotalKwh == 20m;
            Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {label}: {actual.ReadingCount} readings, {actual.TotalKwh} kWh");
            if (!ok) failures++;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL {label}: unexpected {ex.GetType().Name}: {ex.Message}");
            failures++;
        }
    }

    void CheckReject(string label, string xml)
    {
        try
        {
            var actual = CertificateImporter.Import(xml);
            Console.WriteLine($"FAIL {label}: silently accepted as {actual.ReadingCount} readings.");
            failures++;
        }
        catch (InvalidDataException)
        {
            Console.WriteLine($"PASS {label}: rejected with InvalidDataException.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL {label}: wrong exception {ex.GetType().Name}.");
            failures++;
        }
    }
}
