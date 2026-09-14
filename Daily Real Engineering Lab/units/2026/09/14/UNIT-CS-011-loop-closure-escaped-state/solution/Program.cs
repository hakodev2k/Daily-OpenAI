var regions = new[] { "north", "central", "south" };
var planned = new List<Func<string>>();

for (var i = 0; i < regions.Length; i++)
{
    var region = regions[i];
    planned.Add(() => $"processed:{region}");
}

foreach (var work in planned)
{
    Console.WriteLine(work());
}
