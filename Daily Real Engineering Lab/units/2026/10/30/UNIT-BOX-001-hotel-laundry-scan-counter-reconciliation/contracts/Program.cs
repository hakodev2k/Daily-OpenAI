using LaundryAudit;

if (args.Length != 1 || (args[0] != "--reproduce" && args[0] != "--verify"))
{
    Console.Error.WriteLine("Usage: --reproduce | --verify");
    return 2;
}

var morning = ShiftService.CloseShift(new[]
{
    new LaundryScan("BAG-101", true),
    new LaundryScan("BAG-102", true),
    new LaundryScan("BAG-101", true),
    new LaundryScan("BAG-103", true),
    new LaundryScan("BAG-104", false)
});
var afternoon = ShiftService.CloseShift(new[]
{
    new LaundryScan("BAG-201", true),
    new LaundryScan("BAG-201", true),
    new LaundryScan("BAG-202", false)
});
var empty = ShiftService.CloseShift(Array.Empty<LaundryScan>());

Console.WriteLine($"morning: processed={morning.AcceptedByScanner} worker={morning.WorkerCount} report={morning.ReportCount}");
Console.WriteLine($"afternoon: processed={afternoon.AcceptedByScanner} worker={afternoon.WorkerCount} report={afternoon.ReportCount}");
Console.WriteLine($"empty: processed={empty.AcceptedByScanner} worker={empty.WorkerCount} report={empty.ReportCount}");

var inputContract =
    morning.AcceptedByScanner == 3 && morning.WorkerCount == 3 &&
    afternoon.AcceptedByScanner == 1 && afternoon.WorkerCount == 1 &&
    empty.AcceptedByScanner == 0 && empty.WorkerCount == 0 &&
    empty.ReportCount == 0;

if (args[0] == "--reproduce")
{
    if (!inputContract || morning.ReportCount != 0 || afternoon.ReportCount != 0)
    {
        Console.Error.WriteLine("NOT REPRODUCED: starter symptom differs from fixture.");
        return 1;
    }

    Console.WriteLine("REPRODUCED: worker totals and end-of-shift reports disagree.");
    return 0;
}

if (!inputContract || morning.ReportCount != 3 || afternoon.ReportCount != 1)
{
    Console.Error.WriteLine("FAILED: reconciliation contract or isolation checks did not pass.");
    return 1;
}

Console.WriteLine("VERIFIED: worker, report and accepted totals agree across independent shifts.");
return 0;
