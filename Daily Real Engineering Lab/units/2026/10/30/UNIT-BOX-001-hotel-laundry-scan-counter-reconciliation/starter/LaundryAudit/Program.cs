using LaundryAudit;

var summary = ShiftService.CloseShift(new[]
{
    new LaundryScan("BAG-101", true),
    new LaundryScan("BAG-102", true),
    new LaundryScan("BAG-101", true),
    new LaundryScan("BAG-103", true),
    new LaundryScan("BAG-104", false)
});

Console.WriteLine($"processed={summary.AcceptedByScanner} worker={summary.WorkerCount} report={summary.ReportCount}");
