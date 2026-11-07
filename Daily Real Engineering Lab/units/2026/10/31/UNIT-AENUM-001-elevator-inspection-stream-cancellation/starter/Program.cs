using ElevatorScanLab;

var mode = args.FirstOrDefault(x => x.StartsWith("--mode=", StringComparison.Ordinal))?
    .Substring("--mode=".Length) ?? "run";

if (mode is not ("run" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: --mode=run|reproduce|verify");
    return 2;
}

using var stop = new CancellationTokenSource();
var stream = new ShipmentScanStream();
var received = new List<int>();
var cancellationObserved = false;

try
{
    await foreach (var receipt in stream.ReadAsync().WithCancellation(stop.Token))
    {
        received.Add(receipt.Sequence);
        if (received.Count == 1)
        {
            stop.Cancel();
        }
    }
}
catch (OperationCanceledException) when (stop.IsCancellationRequested)
{
    cancellationObserved = true;
}

var passed = cancellationObserved && received.Count == 1 && received[0] == 1;
Console.WriteLine($"RESULT={(passed ? "PASS" : "FAIL")}; cancellationObserved={cancellationObserved}; received={string.Join(",", received)}");

return mode switch
{
    "reproduce" => passed ? 1 : 0,
    "verify" => passed ? 0 : 1,
    _ => 0
};
