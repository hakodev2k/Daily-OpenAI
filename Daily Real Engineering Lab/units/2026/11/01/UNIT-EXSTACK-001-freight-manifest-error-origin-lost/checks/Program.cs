using FreightManifest;

var mode = args.Length == 1 ? args[0] : "demo";
if (mode is not ("demo" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run --project checks/Verifier.csproj -- [demo|reproduce|verify]");
    return 2;
}

try
{
    var audit = new InMemoryImportAudit();
    var runner = new BatchRunner(new ImportCoordinator(new SourceParser(), audit));

    var accepted = runner.Execute("BL-481|CT-903|12");
    if (accepted.PackageCount != 12 || accepted.ContainerId != "CT-903" || audit.RejectedCount != 0)
        throw new Exception("Valid-manifest behavior regressed.");

    FormatException? failure = null;
    try
    {
        runner.Execute("BL-482|CT-904|not-a-number");
    }
    catch (FormatException ex)
    {
        failure = ex;
    }

    if (failure is null)
        throw new Exception("Invalid manifest did not surface the expected FormatException.");
    if (audit.RejectedCount != 1)
        throw new Exception($"Expected exactly one audit rejection, got {audit.RejectedCount}.");

    var stack = failure.StackTrace ?? "";
    var hasCoordinator = stack.Contains("ImportCoordinator.Import", StringComparison.Ordinal);
    var hasParserOrigin = stack.Contains("SourceParser.Parse", StringComparison.Ordinal);

    Console.WriteLine("Exception type: " + failure.GetType().Name);
    Console.WriteLine("Exception message: " + failure.Message);
    Console.WriteLine("Audit rejection count: " + audit.RejectedCount);
    Console.WriteLine("Coordinator frame present: " + hasCoordinator);
    Console.WriteLine("Parser frame present: " + hasParserOrigin);
    Console.WriteLine("Stack trace:");
    Console.WriteLine(stack);

    if (!hasCoordinator)
        throw new Exception("Coordinator stack frame is unexpectedly absent.");

    if (mode == "reproduce" && hasParserOrigin)
    {
        Console.Error.WriteLine("REPRODUCE FAILED: original diagnostic symptom is no longer present.");
        return 1;
    }
    if (mode == "verify" && !hasParserOrigin)
    {
        Console.Error.WriteLine("VERIFY FAILED: original failure origin is still missing.");
        return 1;
    }

    Console.WriteLine(mode == "reproduce"
        ? "REPRODUCED: business rejection remains correct, but origin frame is missing."
        : mode == "verify"
            ? "VERIFIED: exception provenance and business/audit contracts are preserved."
            : "DEMO complete. Use reproduce/verify for explicit checks.");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine("CHECK FAILED: " + ex);
    return 1;
}
