var count = args.Length > 0 ? int.Parse(args[0]) : 40;
var retained = new List<byte[]>();
long peak = 0;
for (var i = 0; i < count; i++)
{
    var buffer = new byte[128 * 1024];
    buffer[0] = (byte)i;
    retained.Add(buffer);
    var live = retained.Sum(x => (long)x.Length);
    peak = Math.Max(peak, live);
}
Console.WriteLine($"documents={count}");
Console.WriteLine($"peakLiveBytes={peak}");
Console.WriteLine($"outputCount={retained.Count}");