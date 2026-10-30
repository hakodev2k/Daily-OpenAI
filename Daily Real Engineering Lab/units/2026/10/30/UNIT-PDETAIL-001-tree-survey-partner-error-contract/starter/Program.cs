using System.Net;
using System.Text;
using System.Text.Json;
using TreeSurveyLab;

var mode = args.FirstOrDefault() ?? "--demo";
return mode switch
{
    "--demo" => await Demo(),
    "--reproduce" => await Reproduce(),
    "--verify" => await Verify(),
    _ => InvalidMode()
};

static int InvalidMode()
{
    Console.Error.WriteLine("Usage: --demo | --reproduce | --verify");
    return 2;
}

static HttpClient CreateHttp() => new(new FakePartnerHandler())
{
    BaseAddress = new Uri("https://partner.test/")
};

static async Task<int> Demo()
{
    using var http = CreateHttp();
    try
    {
        await new PartnerClient(http).SubmitAsync("invalid");
        Console.WriteLine("Unexpected success");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Observed {ex.GetType().Name}: {ex.Message}");
    }
    return 0;
}

static async Task<int> Reproduce()
{
    var observed = 0;
    foreach (var batch in new[] { "invalid", "offline", "legacy" })
    {
        using var http = CreateHttp();
        try
        {
            await new PartnerClient(http).SubmitAsync(batch);
            Console.WriteLine($"FAIL {batch}: unexpectedly accepted");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"OBSERVED {batch}: generic HttpRequestException status={(int?)ex.StatusCode}");
            observed++;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL {batch}: unexpected {ex.GetType().Name}");
        }
    }

    Console.WriteLine($"Observed {observed}/3 intended starter symptoms");
    return observed == 3 ? 0 : 1;
}

static async Task<int> Verify()
{
    var passed = 0;
    passed += await CheckError("invalid", HttpStatusCode.UnprocessableEntity,
        "TREE_PLOT_INVALID", "trace-422", false) ? 1 : 0;
    passed += await CheckError("offline", HttpStatusCode.ServiceUnavailable,
        "PARTNER_MAINTENANCE", "trace-503", true) ? 1 : 0;
    passed += await CheckError("legacy", HttpStatusCode.InternalServerError,
        "UPSTREAM_HTTP_ERROR", "trace-500", true) ? 1 : 0;

    using (var http = CreateHttp())
    {
        try
        {
            var receipt = await new PartnerClient(http).SubmitAsync("ok");
            if (receipt.BatchId == "ok")
            {
                passed++;
                Console.WriteLine("PASS ok: successful batch unchanged");
            }
            else Console.WriteLine("FAIL ok: incorrect receipt");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL ok: {ex.GetType().Name}");
        }
    }

    Console.WriteLine($"Verification: {passed}/4");
    return passed == 4 ? 0 : 1;
}

static async Task<bool> CheckError(
    string batch, HttpStatusCode expectedStatus, string expectedCode,
    string expectedTrace, bool expectedRetryable)
{
    using var http = CreateHttp();
    try
    {
        await new PartnerClient(http).SubmitAsync(batch);
        Console.WriteLine($"FAIL {batch}: expected rejection");
        return false;
    }
    catch (PartnerSyncException ex)
    {
        var ok = ex.StatusCode == expectedStatus
            && ex.ProblemCode == expectedCode
            && ex.TraceId == expectedTrace
            && ex.IsRetryable == expectedRetryable
            && !ex.Message.Contains("Raw upstream details", StringComparison.Ordinal)
            && !ex.Message.Contains("<html>", StringComparison.Ordinal);
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {batch}: status={(int)ex.StatusCode} code={ex.ProblemCode} trace={ex.TraceId} retryable={ex.IsRetryable}");
        return ok;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"FAIL {batch}: unexpected {ex.GetType().Name}");
        return false;
    }
}

internal sealed class FakePartnerHandler : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Method != HttpMethod.Post || request.RequestUri?.AbsolutePath != "/api/batches")
            return new HttpResponseMessage(HttpStatusCode.NotFound);

        var body = await request.Content!.ReadAsStringAsync(cancellationToken);
        using var json = JsonDocument.Parse(body);
        var batch = json.RootElement.GetProperty("batchId").GetString();

        return batch switch
        {
            "ok" => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"receiptId\":\"r-100\"}", Encoding.UTF8, "application/json")
            },
            "invalid" => Problem(HttpStatusCode.UnprocessableEntity, "TREE_PLOT_INVALID", "trace-422"),
            "offline" => Problem(HttpStatusCode.ServiceUnavailable, "PARTNER_MAINTENANCE", "trace-503"),
            "legacy" => Legacy(),
            _ => new HttpResponseMessage(HttpStatusCode.BadRequest)
        };
    }

    private static HttpResponseMessage Problem(HttpStatusCode status, string code, string traceId)
    {
        var json = JsonSerializer.Serialize(new
        {
            type = "https://partner.test/problems/sync",
            title = "Partner rejected batch",
            code,
            traceId,
            detail = "Raw upstream details must not be logged"
        });
        return new HttpResponseMessage(status)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/problem+json")
        };
    }

    private static HttpResponseMessage Legacy()
    {
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("<html>temporary server error</html>", Encoding.UTF8, "text/html")
        };
        response.Headers.TryAddWithoutValidation("X-Request-ID", "trace-500");
        return response;
    }
}
