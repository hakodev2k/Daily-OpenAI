var regions = new[] { "north", "central", "south" };
var planned = new List<Func<string>>();

for (var i = 0; i < regions.Length; i++)
{
    planned.Add(() => $"processed:{regions[i]}");
}

try
{
    foreach (var work in planned)
    {
        Console.WriteLine(work());
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"batch-failed:{ex.GetType().Name}");
    Environment.ExitCode = 2;
}
