using System.Collections.Generic;
using System.Linq;

var objects = new Dictionary<ExhibitKey, string>();
var accession = new ExhibitKey("HALL-A", "OBJ-042");
objects.Add(accession, "Loaned bronze compass");
Console.WriteLine($"EVENT registered {accession.Hall}/{accession.Code}; count={objects.Count}");
accession.Hall = "HALL-B"; // curator moved the exhibit to a new hall
Console.WriteLine($"EVENT relocated {accession.Hall}/{accession.Code}; count={objects.Count}");
Console.WriteLine($"LOOKUP same-reference={objects.TryGetValue(accession, out var byReference)}");
Console.WriteLine($"LOOKUP fresh-key={objects.TryGetValue(new ExhibitKey("HALL-B", "OBJ-042"), out var byValue)}");
Console.WriteLine($"AUDIT stored-keys={string.Join(",", objects.Keys.Select(k => k.Hall + "/" + k.Code))}");
Console.WriteLine($"RESULT found={(byValue is null ? "NO" : "YES")}");

public sealed class ExhibitKey : IEquatable<ExhibitKey>
{
    public string Hall { get; set; }
    public string Code { get; }
    public ExhibitKey(string hall, string code) { Hall = hall; Code = code; }
    public bool Equals(ExhibitKey? other) => other is not null && Hall == other.Hall && Code == other.Code;
    public override bool Equals(object? obj) => Equals(obj as ExhibitKey);
    public override int GetHashCode() => HashCode.Combine(Hall, Code);
}
