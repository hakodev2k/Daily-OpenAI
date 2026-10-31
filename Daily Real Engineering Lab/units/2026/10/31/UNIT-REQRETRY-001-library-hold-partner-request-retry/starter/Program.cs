using System.Net;

namespace PartnerRetryLab;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            if (args.Length != 1 || (args[0] != "--reproduce" && args[0] != "--verify"))
            {
                Console.Error.WriteLine("Usage: dotnet run -- --reproduce | --verify");
                return 2;
            }

            if (args[0] == "--reproduce")
                await ReproduceAsync();
            else
                await VerifyAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FAILED: {ex.GetType().Name}: {ex.Message}");
            return 1;
        }
    }

    private static HttpClient CreateClient(ScriptedPartnerHandler handler) =>
        new(handler) { BaseAddress = new Uri("https://partner.example/") };

    private static async Task ReproduceAsync()
    {
        var handler = new ScriptedPartnerHandler(HttpStatusCode.ServiceUnavailable, HttpStatusCode.Accepted);
        using var http = CreateClient(handler);
        var relay = new PartnerHoldClient(http);
        try
        {
            var result = await relay.SubmitAsync("HOLD-741", CancellationToken.None);
            throw new Exception($"Expected original failure, but returned {result}.");
        }
        catch (InvalidOperationException ex) when (handler.Attempts == 1)
        {
            Console.WriteLine($"REPRODUCED: partner saw {handler.Attempts} request(s); client threw {ex.GetType().Name}.");
        }
    }

    private static async Task VerifyAsync()
    {
        var transient = new ScriptedPartnerHandler(HttpStatusCode.ServiceUnavailable, HttpStatusCode.Accepted);
        using (var http = CreateClient(transient))
        {
            var result = await new PartnerHoldClient(http).SubmitAsync("HOLD-741", CancellationToken.None);
            Require(result == HttpStatusCode.Accepted, "Transient case must return 202.");
            Require(transient.Attempts == 2, "Partner must receive exactly two attempts.");
            foreach (var request in transient.Seen)
            {
                Require(request.Method == "PUT", "Method drift.");
                Require(request.Path == "/holds/HOLD-741", "Path drift.");
                Require(request.Body == "{\"holdId\":\"HOLD-741\"}", "Payload drift.");
                Require(request.IdempotencyKey == "HOLD-741", "Idempotency key drift.");
                Require(request.ContentType == "application/json", "Content type drift.");
            }
            Console.WriteLine("PASS: 503 -> 202, two equivalent partner requests.");
        }

        var permanent = new ScriptedPartnerHandler(HttpStatusCode.BadRequest, HttpStatusCode.Accepted);
        using (var http = CreateClient(permanent))
        {
            await ExpectHttpFailureAsync(new PartnerHoldClient(http), "HOLD-742");
            Require(permanent.Attempts == 1, "400 must not be retried.");
            Console.WriteLine("PASS: 400 does not retry.");
        }

        var immediate = new ScriptedPartnerHandler(HttpStatusCode.Accepted, HttpStatusCode.ServiceUnavailable);
        using (var http = CreateClient(immediate))
        {
            var result = await new PartnerHoldClient(http).SubmitAsync("HOLD-743", CancellationToken.None);
            Require(result == HttpStatusCode.Accepted && immediate.Attempts == 1,
                "Immediate success must not retry.");
            Console.WriteLine("PASS: 202 completes in one attempt.");
        }

        var exhausted = new ScriptedPartnerHandler(HttpStatusCode.ServiceUnavailable, HttpStatusCode.ServiceUnavailable);
        using (var http = CreateClient(exhausted))
        {
            await ExpectHttpFailureAsync(new PartnerHoldClient(http), "HOLD-744");
            Require(exhausted.Attempts == 2, "503 retry must be bounded to two attempts.");
            Console.WriteLine("PASS: repeated 503 stops after two attempts.");
        }

        var canceled = new ScriptedPartnerHandler(HttpStatusCode.Accepted);
        using (var http = CreateClient(canceled))
        using (var cts = new CancellationTokenSource())
        {
            cts.Cancel();
            try
            {
                await new PartnerHoldClient(http).SubmitAsync("HOLD-745", cts.Token);
                throw new Exception("Canceled request unexpectedly succeeded.");
            }
            catch (OperationCanceledException) when (canceled.Attempts == 0)
            {
                Console.WriteLine("PASS: cancellation prevents partner send.");
            }
        }

        Console.WriteLine("VERIFIED: all behavior contracts passed on learner-editable starter.");
    }

    private static async Task ExpectHttpFailureAsync(PartnerHoldClient relay, string id)
    {
        try
        {
            await relay.SubmitAsync(id, CancellationToken.None);
        }
        catch (HttpRequestException)
        {
            return;
        }
        throw new Exception("Expected HttpRequestException for non-success status.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
}

internal sealed record SeenRequest(
    string Method, string Path, string Body, string IdempotencyKey, string ContentType);

internal sealed class ScriptedPartnerHandler : HttpMessageHandler
{
    private readonly HttpStatusCode[] _statuses;
    public List<SeenRequest> Seen { get; } = new();
    public int Attempts => Seen.Count;

    public ScriptedPartnerHandler(params HttpStatusCode[] statuses)
    {
        if (statuses.Length == 0) throw new ArgumentException("Need at least one status.");
        _statuses = statuses;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var body = request.Content is null
            ? "" : await request.Content.ReadAsStringAsync(cancellationToken);
        var key = request.Headers.TryGetValues("Idempotency-Key", out var values)
            ? string.Join(",", values) : "";
        Seen.Add(new SeenRequest(
            request.Method.Method,
            request.RequestUri?.AbsolutePath ?? "",
            body,
            key,
            request.Content?.Headers.ContentType?.MediaType ?? ""));

        var status = _statuses[Math.Min(Seen.Count - 1, _statuses.Length - 1)];
        Console.WriteLine($"PARTNER attempt={Attempts} status={(int)status}");
        return new HttpResponseMessage(status)
        {
            Content = new StringContent(status == HttpStatusCode.Accepted ? "accepted" : "partner-error")
        };
    }
}
