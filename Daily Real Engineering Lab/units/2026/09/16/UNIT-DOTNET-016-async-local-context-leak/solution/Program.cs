using System.Threading;

static class CorrelationContext
{
    public static readonly AsyncLocal<string?> Current = new();
}

static async Task<string?> RunRequestAsync(string id)
{
    CorrelationContext.Current.Value = id;
    Console.WriteLine($"request:{CorrelationContext.Current.Value}");

    Task<string?> detached;
    using (ExecutionContext.SuppressFlow())
    {
        detached = Task.Run(async () =>
        {
            await Task.Delay(25);
            return CorrelationContext.Current.Value;
        });
    }

    CorrelationContext.Current.Value = null;
    return await detached;
}

var value = await RunRequestAsync("req-42");
Console.WriteLine($"detached:{value ?? "<null>"}");