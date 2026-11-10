using System.Text;
var r = new Registry();
string a = "CAF\u00C9-042", b = "CAFE\u0301-042";
r.Import(a, "Vase", "A"); r.Import(b, "Vase", "B");
Console.WriteLine($"Entries={r.Count}; A={r.Find(a)?.Source}; B={r.Find(b)?.Source}");
Console.WriteLine($"A: length={a.Length}, bytes={Convert.ToHexString(Encoding.UTF8.GetBytes(a))}");
Console.WriteLine($"B: length={b.Length}, bytes={Convert.ToHexString(Encoding.UTF8.GetBytes(b))}");
if (args.Contains("--contract")) {
 bool equivalent = r.Count == 1 && r.Find(a) != null && r.Find(b) != null;
 r.Import("CAFE-042", "Different artifact", "C");
 bool distinct = r.Count == 2 && r.Find("CAFE-042")?.Source == "C";
 if (!equivalent || !distinct) { Console.Error.WriteLine($"CONTRACT_FAIL equivalent={equivalent} distinct={distinct}"); Environment.ExitCode=1; }
 else Console.WriteLine("CONTRACT_PASS");
}
public sealed record Artifact(string Id,string Title,string Source);
public sealed class Registry {
 private readonly Dictionary<string,Artifact> items = new(StringComparer.Ordinal);
 public int Count => items.Count;
 public void Import(string id,string title,string source) {
  if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Missing ID",nameof(id));
  items[id]=new Artifact(id,title,source);
 }
 public Artifact? Find(string id) => items.TryGetValue(id,out var a)?a:null;
}
