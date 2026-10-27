using System.Net;
using System.Text;
using System.Text.Json;

namespace BulkIngestionLab;

public sealed record SearchDocument(string Id, string RouteName);
public sealed record IngestionReport(
    IReadOnlyList<string> IndexedIds,
    IReadOnlyList<string> QuarantinedIds,
    IReadOnlyList<string> RetryExhaustedIds);

/// <summary>Deterministic fake of the per-item response contract, not an Elasticsearch server.</summary>
public sealed class FakeSearchGateway
{
    private readonly Dictionary<string, int> _attempts = new(StringComparer.Ordinal);
    public IReadOnlyDictionary<string, int> Attempts => _attempts;
    public List<string> ActuallyIndexedIds { get; } = new();

    public Task<HttpResponseMessage> SendBulkAsync(
        IReadOnlyList<SearchDocument> batch, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var outcomes = new List<(string Id, int Status)>();

        foreach (var doc in batch)
        {
            var attempt = _attempts.GetValueOrDefault(doc.Id) + 1;
            _attempts[doc.Id] = attempt;
            var status = doc.Id switch
            {
                "route-102" when attempt == 1 => 429,
                "route-103" => 400,
                _ => 201
            };
            if (status == 201)
                ActuallyIndexedIds.Add(doc.Id);
            outcomes.Add((doc.Id, status));
        }

        var items = outcomes.Select(o => new
        {
            index = new
            {
                _id = o.Id,
                status = o.Status,
                error = o.Status < 300 ? null : new
                {
                    type = o.Status == 429
                        ? "es_rejected_execution_exception"
                        : "mapper_parsing_exception",
                    reason = o.Status == 429
                        ? "Synthetic write queue temporarily full"
                        : "Synthetic invalid geo_point"
                }
            }
        }).ToArray();
        var body = JsonSerializer.Serialize(new
        {
            took = 7,
            errors = outcomes.Any(o => o.Status >= 300),
            items
        });
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
        return Task.FromResult(response);
    }
}

public static class Scenario
{
    public static async Task<int> Main()
    {
        SearchDocument[] documents =
        [
            new("route-101", "North Connector"),
            new("route-102", "Coastal Express"),
            new("route-103", "Mountain Shuttle"),
            new("route-104", "Valley Local")
        ];
        var gateway = new FakeSearchGateway();
        var report = await new BulkIngestor().IngestAsync(documents, gateway, CancellationToken.None);

        static string Join(IEnumerable<string> ids) =>
            string.Join(",", ids.OrderBy(x => x, StringComparer.Ordinal));
        Console.WriteLine("HTTP_STATUS=200");
        Console.WriteLine("INDEXED_REPORTED=" + Join(report.IndexedIds));
        Console.WriteLine("INDEXED_ACTUAL=" + Join(gateway.ActuallyIndexedIds));
        Console.WriteLine("QUARANTINED=" + Join(report.QuarantinedIds));
        Console.WriteLine("RETRY_EXHAUSTED=" + Join(report.RetryExhaustedIds));
        Console.WriteLine("ATTEMPTS=" + string.Join(",",
            gateway.Attempts.OrderBy(x => x.Key, StringComparer.Ordinal)
                .Select(x => $"{x.Key}:{x.Value}")));

        string[] expectedIndexed = ["route-101", "route-102", "route-104"];
        var valid =
            Join(report.IndexedIds) == Join(expectedIndexed) &&
            Join(gateway.ActuallyIndexedIds) == Join(expectedIndexed) &&
            Join(report.QuarantinedIds) == "route-103" &&
            report.RetryExhaustedIds.Count == 0 &&
            gateway.Attempts.Count == 4 &&
            gateway.Attempts.GetValueOrDefault("route-101") == 1 &&
            gateway.Attempts.GetValueOrDefault("route-102") == 2 &&
            gateway.Attempts.GetValueOrDefault("route-103") == 1 &&
            gateway.Attempts.GetValueOrDefault("route-104") == 1;

        if (!valid)
        {
            Console.Error.WriteLine("CHECK_FAILED: reported/actual index, quarantine or retry invariant violated");
            return 2;
        }
        Console.WriteLine("CHECK_PASSED: all per-item outcomes accounted for");
        return 0;
    }
}
