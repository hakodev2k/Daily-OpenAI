var invoices = new[]
{
    new Invoice("INV-A", priority: 1, dueDate: new DateOnly(2026, 9, 12)),
    new Invoice("INV-B", priority: 2, dueDate: new DateOnly(2026, 9, 11)),
    new Invoice("INV-C", priority: 1, dueDate: new DateOnly(2026, 9, 10))
};

var ordered = invoices
    .OrderBy(x => x.Priority)
    .ThenBy(x => x.DueDate)
    .ToArray();

Console.WriteLine($"COUNT={ordered.Length}");
Console.WriteLine($"ORDER={string.Join(',', ordered.Select(x => x.Id))}");

internal sealed record Invoice(string Id, int Priority, DateOnly DueDate);
