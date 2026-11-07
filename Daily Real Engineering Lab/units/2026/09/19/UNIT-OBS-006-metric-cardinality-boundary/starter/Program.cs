var series = new HashSet<string>(StringComparer.Ordinal);
var routes = new[] { "/orders/{id}", "/orders/search" };

for (var i = 1; i <= 120; i++)
{
    var route = routes[i % routes.Length];
    var status = i % 10 == 0 ? "5xx" : "2xx";
    var requestId = $"req-{i:D4}";

    // Investigation note: each value below becomes part of the metric series identity.
    RecordRequestMetric(series, route, status, requestId);
}

Console.WriteLine($"requests=120");
Console.WriteLine($"routes={routes.Length}");
Console.WriteLine($"series={series.Count}");

if (args.Contains("reproduce"))
{
    if (series.Count >= 100) { Console.WriteLine("REPRODUCED"); return 2; }
    Console.WriteLine("NOT_REPRODUCED"); return 1;
}

if (args.Contains("verify"))
{
    if (series.Count <= 8) { Console.WriteLine("VERIFY_PASS"); return 0; }
    Console.WriteLine("VERIFY_FAIL"); return 3;
}

return 0;

static void RecordRequestMetric(HashSet<string> series, string route, string statusClass, string requestId)
{
    series.Add($"http.server.duration|route={route}|status={statusClass}|request_id={requestId}");
}
