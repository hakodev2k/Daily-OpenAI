using System.Text.Json;
using MuseumConditionLab;

return args.Length == 1 ? args[0] switch
{
    "--reproduce" => Reproduce(),
    "--verify" => Verify(),
    _ => Usage()
} : Usage();

static int Usage()
{
    Console.Error.WriteLine("Usage: dotnet run --project starter/ConditionLab.csproj -- --reproduce|--verify");
    return 2;
}

static int Reproduce()
{
    var report = new ConditionReport("MUSE-204", 48, "stable");
    try
    {
        var first = ConditionReportExporter.Export(report, pretty: false);
        using var parsed = JsonDocument.Parse(first);
        Console.WriteLine("First export succeeded: " + parsed.RootElement.GetProperty("exhibitId").GetString());
        var second = ConditionReportExporter.Export(report, pretty: true);
        Console.Error.WriteLine("NOT REPRODUCED: second export succeeded: " + second);
        return 1;
    }
    catch (InvalidOperationException ex) when (
        ex.Message.Contains("read-only", StringComparison.OrdinalIgnoreCase) ||
        ex.Message.Contains("already been used", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("REPRODUCED: " + ex.Message);
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("UNEXPECTED FAILURE: " + ex);
        return 2;
    }
}

static int Verify()
{
    var a = new ConditionReport("MUSE-204", 48, "stable");
    var b = new ConditionReport("MUSE-205", 71, "review");
    try
    {
        var compactA = ConditionReportExporter.Export(a, pretty: false);
        var prettyB = ConditionReportExporter.Export(b, pretty: true);
        var compactB = ConditionReportExporter.Export(b, pretty: false);
        var prettyA = ConditionReportExporter.Export(a, pretty: true);
        Check(compactA, a);
        Check(prettyB, b);
        Check(compactB, b);
        Check(prettyA, a);
        if (compactA.Contains('\n') || compactB.Contains('\n') ||
            !prettyA.Contains('\n') || !prettyB.Contains('\n'))
            throw new Exception("Formatting contract violated.");
        Console.WriteLine("PASS: four alternating exports, camelCase payloads, independent formatting.");
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("FAIL: " + ex);
        return 1;
    }
}

static void Check(string json, ConditionReport expected)
{
    using var parsed = JsonDocument.Parse(json);
    var obj = parsed.RootElement;
    if (obj.GetProperty("exhibitId").GetString() != expected.ExhibitId ||
        obj.GetProperty("humidityPercent").GetInt32() != expected.HumidityPercent ||
        obj.GetProperty("status").GetString() != expected.Status)
        throw new Exception("Payload contract violated: " + json);
}
