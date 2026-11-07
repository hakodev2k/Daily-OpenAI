record Item(string Sku, int Quantity);
var source = new List<Item> { new("A-100", 4), new("B-200", 3), new("C-300", 0) };
IEnumerable<Item> available = source.Where(x => x.Quantity > 0);
var summaryCount = available.Count();
Console.WriteLine($"summary-count={summaryCount}");
source.Add(new Item("D-400", 2));
var exported = available.Select(x => x.Sku).ToArray();
Console.WriteLine($"export={string.Join(",", exported)}");
if (args.Contains("reproduce") && summaryCount != exported.Length) { Console.WriteLine("REPRODUCED"); return 2; }
if (args.Contains("verify")) { if (summaryCount == exported.Length) { Console.WriteLine("VERIFY_PASS"); return 0; } Console.WriteLine("VERIFY_FAIL"); return 3; }
return 0;