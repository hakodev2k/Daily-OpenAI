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

public sealed class ExportRequest : IEquatable<ExportRequest>
{
    public ExportRequest(string customerId, IReadOnlyList<string> columns)
    {
        CustomerId = customerId;
        Columns = columns;
    }

    public string CustomerId { get; }
    public IReadOnlyList<string> Columns { get; }

    public bool Equals(ExportRequest? other)
    {
        return other is not null
            && StringComparer.Ordinal.Equals(CustomerId, other.CustomerId)
            && Columns.SequenceEqual(other.Columns, StringComparer.Ordinal);
    }

    public override bool Equals(object? obj) => obj is ExportRequest other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(CustomerId, StringComparer.Ordinal);
        foreach (var column in Columns)
        {
            hash.Add(column, StringComparer.Ordinal);
        }
        return hash.ToHashCode();
    }
}
