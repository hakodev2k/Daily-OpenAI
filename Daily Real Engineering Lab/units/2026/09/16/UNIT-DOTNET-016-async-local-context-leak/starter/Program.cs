using System.Threading;

static class CorrelationContext
{
    public static readonly AsyncLocal<string?> Current = new();
}

static async Task<string?> RunRequestAsync(string id)
{
    CorrelationContext.Current.Value = id;
    Console.WriteLine($"request:{CorrelationContext.Current.Value}");

    var detached = Task.Run(async () =>
    {
        await Task.Delay(25);
        return CorrelationContext.Current.Value;
    });

    CorrelationContext.Current.Value = null;
    return await detached;
}

var leaked = await RunRequestAsync("req-42");
Console.WriteLine($"detached:{leaked ?? "<null>"}");

if (args.Contains("--verify", StringComparer.OrdinalIgnoreCase))
{
    if (leaked is not null)
        throw new InvalidOperationException("Detached work inherited request correlation context.");

    Console.WriteLine("VERIFY_PASS");
}