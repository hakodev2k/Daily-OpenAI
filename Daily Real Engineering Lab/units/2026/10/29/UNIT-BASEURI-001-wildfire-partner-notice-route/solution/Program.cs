using System.Net;
using System.Net.Http;

return await LabRunner.RunAsync(args.Length == 0 ? "run" : args[0]);

internal static class PartnerRequests
{
    public static HttpRequestMessage BuildNoticeRequest(string district)
    {
        var districtQuery = Uri.EscapeDataString(district);
        return new HttpRequestMessage(HttpMethod.Get, $"notices?district={districtQuery}");
    }
}

internal static class LabRunner
{
    private sealed record Case(string Name, string BaseUrl, string District);
    private sealed record Observation(string Name, string RequestUri, HttpStatusCode Status);

    private static readonly Case[] Cases =
    [
        new("primary-v3", "https://dispatch.partner.invalid/v3/", "NW-4"),
        new("backup-nested-v3", "https://backup.partner.invalid/integrations/v3/", "NW-4"),
        new("legacy-root-control", "https://legacy.partner.invalid/", "NW-4")
    ];

    public static async Task<int> RunAsync(string mode)
    {
        if (mode is not ("run" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Use: run | reproduce | verify");
            return 2;
        }

        var results = new List<Observation>();
        foreach (var test in Cases)
        {
            var baseUri = new Uri(test.BaseUrl, UriKind.Absolute);
            using var handler = new ContractHandler(baseUri, test.District);
            using var client = new HttpClient(handler) { BaseAddress = baseUri };
            using var request = PartnerRequests.BuildNoticeRequest(test.District);
            using var response = await client.SendAsync(request);
            results.Add(new Observation(test.Name, handler.LastUri ?? "(none)", response.StatusCode));
        }

        foreach (var result in results)
            Console.WriteLine($"{result.Name,-20} status={(int)result.Status} uri={result.RequestUri}");

        var baseline =
            results[0].Status == HttpStatusCode.NotFound &&
            results[1].Status == HttpStatusCode.NotFound &&
            results[2].Status == HttpStatusCode.OK;
        var valid = results.All(x => x.Status == HttpStatusCode.OK);

        if (mode == "reproduce")
        {
            Console.WriteLine(baseline ? "REPRODUCED: baseline discrepancy." : "NOT REPRODUCED.");
            return baseline ? 0 : 1;
        }
        if (mode == "verify")
        {
            Console.WriteLine(valid ? "PASS: all partner routes." : "FAIL: route contract.");
            return valid ? 0 : 1;
        }
        return 0;
    }

    private sealed class ContractHandler(Uri baseUri, string district) : HttpMessageHandler
    {
        public string? LastUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LastUri = request.RequestUri?.AbsoluteUri;
            var expected = new Uri(baseUri, $"notices?district={Uri.EscapeDataString(district)}");
            var matched = request.Method == HttpMethod.Get &&
                string.Equals(LastUri, expected.AbsoluteUri, StringComparison.Ordinal);
            return Task.FromResult(new HttpResponseMessage(
                matched ? HttpStatusCode.OK : HttpStatusCode.NotFound)
            {
                Content = new StringContent(matched
                    ? "{\"count\":2,\"accepted\":true}"
                    : "{\"error\":\"route-not-found\"}")
            });
        }
    }
}
