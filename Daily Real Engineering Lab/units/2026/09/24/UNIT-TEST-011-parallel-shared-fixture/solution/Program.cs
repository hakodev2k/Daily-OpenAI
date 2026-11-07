using System.Collections.Concurrent;
var failures=new ConcurrentBag<string>();
for(var round=0;round<40;round++){var a=RunCase("A","SKU-A",3,failures);var b=RunCase("B","SKU-B",7,failures);await Task.WhenAll(a,b);}
Console.WriteLine(failures.IsEmpty?"PASS":$"FAILURES={failures.Count}");
if(!failures.IsEmpty) Environment.ExitCode=1;
static async Task RunCase(string test,string sku,int quantity,ConcurrentBag<string> failures){var inventory=new TestInventory();inventory.Seed(sku,quantity);await Task.Delay(test=="A"?8:4);var observed=inventory.Get(sku);if(observed!=quantity) failures.Add($"{test}: expected {quantity}, observed {observed}");}
sealed class TestInventory{private readonly Dictionary<string,int> rows=new();public void Seed(string sku,int quantity)=>rows[sku]=quantity;public int Get(string sku)=>rows.TryGetValue(sku,out var value)?value:0;}