using System.Diagnostics;

using var requestAborted = new CancellationTokenSource();
requestAborted.CancelAfter(TimeSpan.FromMilliseconds(150));

var stopwatch = Stopwatch.StartNew();

try
{
    Console.WriteLine("Request started.");
    await LoadCustomerProfileAsync("CUS-001", requestAborted.Token);
    Console.WriteLine($"Request completed after {stopwatch.ElapsedMilliseconds} ms.");
}
catch (OperationCanceledException) when (requestAborted.IsCancellationRequested)
{
    Console.WriteLine($"Request canceled after {stopwatch.ElapsedMilliseconds} ms.");
}

static async Task LoadCustomerProfileAsync(string customerId, CancellationToken cancellationToken)
{
    Console.WriteLine($"Loading {customerId}...");
    await FetchDownstreamProfileAsync(customerId, cancellationToken);
}

static async Task FetchDownstreamProfileAsync(string customerId, CancellationToken cancellationToken)
{
    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    Console.WriteLine($"Downstream completed for {customerId}.");
}
