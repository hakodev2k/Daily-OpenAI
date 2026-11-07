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

static async IAsyncEnumerable<string> ExportAsync(int count, [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    await foreach (var row in ReadRowsAsync(count, cancellationToken))
        yield return row;
}

var received = 0;
await foreach (var row in ExportAsync(count))
{
    received++;
    Console.WriteLine($"CLIENT_RECEIVED {received} {row}");
}

Console.WriteLine($"TOTAL_RECEIVED {received}");