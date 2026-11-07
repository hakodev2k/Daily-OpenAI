var tenants = new[] { "alpha", "bravo", "charlie" };
var callbacks = new List<Func<Task<string>>>();

for (var i = 0; i < tenants.Length; i++)
{
    callbacks.Add(async () =>
    {
        await Task.Yield();
        return tenants[i];
    });
}

try
{
    var results = await Task.WhenAll(callbacks.Select(callback => callback()));
    var ordered = results.OrderBy(x => x).ToArray();
    Console.WriteLine(string.Join(",", ordered));
    return ordered.SequenceEqual(tenants.OrderBy(x => x)) ? 0 : 43;
}
catch (Exception ex)
{
    Console.WriteLine($"Observed failure: {ex.GetType().Name}: {ex.Message}");
    return 42;
}
