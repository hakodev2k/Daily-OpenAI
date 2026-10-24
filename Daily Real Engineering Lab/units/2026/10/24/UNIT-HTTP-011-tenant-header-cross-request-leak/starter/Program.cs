using System.Collections.Concurrent;
using System.Net;
using OutboundTenantLab;

return await LabRunner.RunAsync(args);

internal static class LabRunner
{
    public static async Task<int> RunAsync(string[] args)
    {
        var mode = args.FirstOrDefault()?.ToLowerInvariant() ?? "run";
        if (mode is not ("run" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run -- [run|reproduce|verify]");
            return 2;
        }

        var serial = await ExecuteAsync(overlap: false);
        var overlapping = await ExecuteAsync(overlap: true);
        Print("serial", serial);
        Print("overlap", overlapping);

        var serialCorrect = IsCorrect(serial);
        var overlapCorrect = IsCorrect(overlapping);
        Console.WriteLine($"serialCorrect={serialCorrect}; overlapCorrect={overlapCorrect}");

        return mode switch
        {
            "run" => 0,
            "reproduce" when serialCorrect && !overlapCorrect => 0,
            "verify" when serialCorrect && overlapCorrect => 0,
            _ => 1
        };
    }

    private static async Task<Dictionary<string, string>> ExecuteAsync(bool overlap)
    {
        using var handler = new RecordingHandler();
        using var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://partner.example.invalid/")
        };
        var client = new PartnerClient(http);

        if (!overlap)
        {
            await client.SubmitAsync("tenant-alpha", "order-a", () => Task.CompletedTask);
            await client.SubmitAsync("tenant-beta", "order-b", () => Task.CompletedTask);
        }
        else
        {
            var aReady = NewSignal();
            var bReady = NewSignal();

            var a = client.SubmitAsync("tenant-alpha", "order-a", async () =>
            {
                aReady.TrySetResult();
                await bReady.Task;
            });

            await aReady.Task.WaitAsync(TimeSpan.FromSeconds(5));

            var b = client.SubmitAsync("tenant-beta", "order-b", () =>
            {
                bReady.TrySetResult();
                return Task.CompletedTask;
            });

            await Task.WhenAll(a, b).WaitAsync(TimeSpan.FromSeconds(5));
        }

        return handler.Snapshot();
    }

    private static TaskCompletionSource NewSignal() =>
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static bool IsCorrect(IReadOnlyDictionary<string, string> seen) =>
        seen.Count == 2 &&
        seen.TryGetValue("/orders/order-a", out var a) && a == "tenant-alpha" &&
        seen.TryGetValue("/orders/order-b", out var b) && b == "tenant-beta";

    private static void Print(string label, IReadOnlyDictionary<string, string> seen)
    {
        foreach (var (order, expected) in new[]
        {
            ("/orders/order-a", "tenant-alpha"),
            ("/orders/order-b", "tenant-beta")
        })
        {
            seen.TryGetValue(order, out var actual);
            Console.WriteLine($"{label} {order}: expected={expected}, actual={actual ?? "<missing>"}");
        }
    }
}

internal sealed class RecordingHandler : HttpMessageHandler
{
    private readonly ConcurrentDictionary<string, string> _seen = new();

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tenant = request.Headers.TryGetValues("X-Tenant-Id", out var values)
            ? string.Join(",", values)
            : "<missing>";
        _seen[request.RequestUri!.AbsolutePath] = tenant;
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Accepted));
    }

    public Dictionary<string, string> Snapshot() => new(_seen);
}
