using System.Collections.Generic;
using System.Linq;

var objects = new Dictionary<ExhibitKey, string>();
var accession = new ExhibitKey("HALL-A", "OBJ-042");
objects.Add(accession, "Loaned bronze compass");
Console.WriteLine($"EVENT registered {accession.Hall}/{accession.Code}; count={objects.Count}");
// Relocation changes the identity used for lookup; remove the old identity first.
var oldKey = accession;
if (!objects.Remove(oldKey, out var description)) throw new InvalidOperationException("Missing old key");
accession = new ExhibitKey("HALL-B", oldKey.Code);
objects.Add(accession, description);
Console.WriteLine($"EVENT relocated {accession.Hall}/{accession.Code}; count={objects.Count}");
Console.WriteLine($"LOOKUP same-reference={objects.TryGetValue(accession, out var byReference)}");
Console.WriteLine($"LOOKUP fresh-key={objects.TryGetValue(new ExhibitKey("HALL-B", "OBJ-042"), out var byValue)}");
Console.WriteLine($"AUDIT stored-keys={string.Join(",", objects.Keys.Select(k => k.Hall + "/" + k.Code))}");
Console.WriteLine($"RESULT found={(byValue is null ? "NO" : "YES")}");

public sealed record ExhibitKey(string Hall, string Code);
