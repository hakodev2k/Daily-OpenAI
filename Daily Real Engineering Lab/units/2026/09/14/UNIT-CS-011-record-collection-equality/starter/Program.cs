var first = new ExportRequest("customer-42", new[] { "id", "email" });
var equivalent = new ExportRequest("customer-42", new[] { "id", "email" });
var different = new ExportRequest("customer-42", new[] { "id", "phone" });

var dedupe = new HashSet<ExportRequest>();
dedupe.Add(first);
dedupe.Add(equivalent);

Console.WriteLine($"first-equals-equivalent:{first.Equals(equivalent)}");
Console.WriteLine($"equivalent-count:{dedupe.Count}");

dedupe.Add(different);
Console.WriteLine($"total-count:{dedupe.Count}");

public record ExportRequest(string CustomerId, IReadOnlyList<string> Columns);
