using System.Net;
using System.Text;
using CanalGate.Export;

if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- <reproduce|verify>");
    return 2;
}

try
{
    if (args[0] == "reproduce")
    {
        using var handler = new TrackingHandler();
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://lab.invalid/") };
        var client = new SensorExportClient(http);
        await using var stream = await client.OpenExportAsync("canal-17");

        try
        {
            _ = stream.ReadByte();
            Console.Error.WriteLine("Reproduce FAILED: the original read failure was not observed.");
            return 1;
        }
        catch (ObjectDisposedException)
        {
            Check(handler.LastStream?.IsDisposed == true,
                "Expected tracking evidence that the content stream was already disposed.");
            Console.WriteLine("Reproduced: HTTP 200, then ObjectDisposedException on first payload read.");
            return 0;
        }
    }

    using (var handler = new TrackingHandler())
    using (var http = new HttpClient(handler) { BaseAddress = new Uri("https://lab.invalid/") })
    {
        var client = new SensorExportClient(http);
        await using (var stream = await client.OpenExportAsync("canal-17"))
        {
            Check(handler.LastStream is not null, "No response stream was created.");
            Check(!handler.LastStream.IsDisposed,
                "Stream must remain readable after OpenExportAsync returns.");

            using var reader = new StreamReader(
                stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024, leaveOpen: true);
            var csv = await reader.ReadToEndAsync();
            Check(csv == TrackingHandler.ExpectedCsv, "CSV content mismatch.");
            Check(!handler.LastStream.IsDisposed,
                "Caller has not disposed the stream yet.");
        }

        Check(handler.LastStream!.IsDisposed,
            "Closing the returned stream must release the HTTP content.");
    }

    using (var handler = new TrackingHandler { StatusCode = HttpStatusCode.ServiceUnavailable })
    using (var http = new HttpClient(handler) { BaseAddress = new Uri("https://lab.invalid/") })
    {
        var client = new SensorExportClient(http);
        try
        {
            await using var unused = await client.OpenExportAsync("canal-17");
            throw new InvalidOperationException("HTTP 503 must not return a successful stream.");
        }
        catch (HttpRequestException)
        {
            Check(handler.LastStream?.IsDisposed == true,
                "HTTP failure must release its response content.");
        }
    }

    using (var handler = new TrackingHandler())
    using (var http = new HttpClient(handler) { BaseAddress = new Uri("https://lab.invalid/") })
    using (var cancellation = new CancellationTokenSource())
    {
        cancellation.Cancel();
        var client = new SensorExportClient(http);
        try
        {
            await using var unused = await client.OpenExportAsync("canal-17", cancellation.Token);
            throw new InvalidOperationException("Canceled call must not return a stream.");
        }
        catch (OperationCanceledException)
        {
            Check(handler.LastStream is null,
                "Pre-canceled call must not create response content.");
        }
    }

    Console.WriteLine("VERIFY PASS: CSV, ownership, HTTP 503 cleanup and cancellation.");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
    return 1;
}

static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
