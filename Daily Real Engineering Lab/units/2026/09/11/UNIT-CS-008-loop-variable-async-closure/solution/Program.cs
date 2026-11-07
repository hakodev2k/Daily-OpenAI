var tenants = new[] { "alpha", "bravo", "charlie" };
var callbacks = new List<Func<Task<string>>>();

for (var i = 0; i < tenants.Length; i++)
{
    var tenant = tenants[i];
    callbacks.Add(async () =>
    {
        await Task.Yield();
        return tenant;
    });
}

var results = await Task.WhenAll(callbacks.Select(callback => callback()));
var ordered = results.OrderBy(x => x).ToArray();
Console.WriteLine(string.Join(",", ordered));
return ordered.SequenceEqual(tenants.OrderBy(x => x)) ? 0 : 1;
