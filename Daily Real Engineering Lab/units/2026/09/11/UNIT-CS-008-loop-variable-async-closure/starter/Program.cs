var tenants = new[] { "alpha", "bravo", "charlie" };
var callbacks = new List<Func<string>>();

for (var i = 0; i < tenants.Length; i++)
{
    callbacks.Add(() => tenants[i]);
}

try
{
    var results = callbacks.Select(callback => callback()).OrderBy(x => x).ToArray();
    Console.WriteLine(string.Join(",", results));
    return results.SequenceEqual(tenants.OrderBy(x => x)) ? 0 : 43;
}
catch (Exception ex)
{
    Console.WriteLine($"Observed failure: {ex.GetType().Name}: {ex.Message}");
    return 42;
}
