using System.Diagnostics;

var worker = new IrrigationWorker();
await worker.RunTwoCyclesAsync(CancellationToken.None);

internal sealed class IrrigationWorker
{
    private readonly CancellationTokenSource _budget = new();
    public async Task RunTwoCyclesAsync(CancellationToken shutdown)
    {
        for (var cycle = 1; cycle <= 2; cycle++)
        {
            var timer = Stopwatch.StartNew();
            _budget.CancelAfter(TimeSpan.FromMilliseconds(120));
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(shutdown, _budget.Token);
            try
            {
                await SimulateValveAsync(linked.Token);
                Console.WriteLine($"cycle={cycle} status=ok elapsedMs={timer.ElapsedMilliseconds}");
            }
            catch (OperationCanceledException) when (linked.IsCancellationRequested)
            {
                Console.WriteLine($"cycle={cycle} status=cancelled elapsedMs={timer.ElapsedMilliseconds}");
            }
            await Task.Delay(160, shutdown);
        }
    }

    private static async Task SimulateValveAsync(CancellationToken token)
    {
        await Task.Delay(25, token);
    }
}
