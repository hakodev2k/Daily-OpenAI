var series = new HashSet<string>();

for (var userId = 1; userId <= 5000; userId++)
{
    var requestPath = $"/api/users/{userId}/orders";
    RecordRequest(requestPath);
}

Console.WriteLine($"request_count=5000");
Console.WriteLine($"series_count={series.Count}");

void RecordRequest(string endpoint)
{
    // This string represents the label-set identity sent to a metrics backend.
    series.Add($"http_requests_total|endpoint={endpoint}|method=GET");
}
