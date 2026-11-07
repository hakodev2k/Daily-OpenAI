static class CorrelationContext
{
    public static readonly AsyncLocal<string?> Current = new();
}

static async Task HandleAsync(string name, string? correlationId)
{
    if (correlationId is not null)
    {
        CorrelationContext.Current.Value = correlationId;
    }

    await Task.Delay(10);
    Console.WriteLine($"{name}: correlation={CorrelationContext.Current.Value ?? "<null>"}");
}

await HandleAsync("operation-A", "corr-A");
await HandleAsync("operation-B", null);
