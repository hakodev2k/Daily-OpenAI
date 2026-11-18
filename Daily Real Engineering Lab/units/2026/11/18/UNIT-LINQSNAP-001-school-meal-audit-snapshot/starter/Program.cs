using System;
using System.Collections.Generic;
using System.Linq;

var meals = new List<Meal> { new(101, "North School", 25), new(102, "East School", 30) };
var report = new MealAuditExporter().Export(meals, () => meals.Add(new Meal(103, "West School", 28)));
Console.WriteLine($"reportedCount={report.ReportedCount} detailCount={report.Details.Count} sourceCount={meals.Count}");
Console.WriteLine(string.Join(",", report.Details.Select(x => x.Id)));
return report.ReportedCount == 2 && report.Details.Count == 2 && meals.Count == 3 ? 0 : 1;

public sealed record Meal(int Id, string School, int Portions);
public sealed record AuditReport(int ReportedCount, IReadOnlyList<Meal> Details);
public sealed class MealAuditExporter
{
    public AuditReport Export(IEnumerable<Meal> source, Action afterSummary)
    {
        var selected = source.Where(x => x.Portions >= 20);
        var reportedCount = selected.Count();
        afterSummary();
        var details = selected.ToList();
        return new AuditReport(reportedCount, details);
    }
}
