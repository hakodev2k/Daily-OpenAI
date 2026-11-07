var items = Enumerable.Range(1, 10).OrderByDescending(x => x).ToList();
const int size = 3;
var page1 = items.Skip(0).Take(size).ToArray();
Console.WriteLine($"PAGE1={string.Join(",", page1)}");
items.Insert(0, 11);
var page2 = items.Skip(size).Take(size).ToArray();
Console.WriteLine($"PAGE2={string.Join(",", page2)}");
var combined = page1.Concat(page2).ToArray();
Console.WriteLine($"COMBINED={string.Join(",", combined)}");
