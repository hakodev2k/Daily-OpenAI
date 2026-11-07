using System.Runtime.CompilerServices;

const int count = 8;

static async IAsyncEnumerable<string> ReadRowsAsync(int count, [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    for (var i = 1; i <= count; i++)
    {
        await Task.Delay(80, cancellationToken);
        Console.WriteLine($"PRODUCED {i}");
        yield return $"{{\"row\":{i}}}";
    }
}

// Investigation note: the public contract is asynchronous, but inspect when
// the consumer is actually allowed to observe the first item.
static async IAsyncEnumerable<string> ExportAsync(int count, [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    var buffered = new List<string>();
    await foreach (var row in ReadRowsAsync(count, cancellationToken))
        buffered.Add(row);

    Console.WriteLine($"PEAK_BUFFERED {buffered.Count}");
    foreach (var row in buffered)
        yield return row;
}

var received = 0;
await foreach (var row in ExportAsync(count))
{
    received++;
    Console.WriteLine($"CLIENT_RECEIVED {received} {row}");
}

Console.WriteLine($"TOTAL_RECEIVED {received}");