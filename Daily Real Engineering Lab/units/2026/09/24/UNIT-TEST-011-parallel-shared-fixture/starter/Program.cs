using System.Collections.Concurrent;

var failures = new ConcurrentBag<string>();
for (var round = 0; round < 40; round++)
{
    var a = RunCase("A", "SKU-A", 3, failures);
    var b = RunCase("B", "SKU-B", 7, failures);
    await Task.WhenAll(a, b);
}

if (!failures.IsEmpty)
{
    Console.WriteLine($"FAILURES={failures.Count}");
    foreach (var failure in failures.Take(5)) Console.WriteLine(failure);
    Environment.ExitCode = 1;
}
else Console.WriteLine("PASS");

static async Task RunCase(string test, string sku, int quantity, ConcurrentBag<string> failures)
{
    SharedInventory.Reset();
    SharedInventory.Seed(sku, quantity);
    await Task.Delay(test == "A" ? 8 : 4);
    var observed = SharedInventory.Get(sku);
    if (observed != quantity) failures.Add($"{test}: expected {sku}={quantity}, observed {observed}");
}

static class SharedInventory
{
    private static readonly ConcurrentDictionary<string,int> Rows = new();
    public static void Reset() => Rows.Clear();
    public static void Seed(string sku, int quantity) => Rows[sku] = quantity;
    public static int Get(string sku) => Rows.TryGetValue(sku, out var value) ? value : 0;
}