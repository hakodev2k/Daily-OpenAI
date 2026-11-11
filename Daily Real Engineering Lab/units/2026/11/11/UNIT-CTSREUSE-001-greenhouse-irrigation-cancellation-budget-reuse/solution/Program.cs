using System.Diagnostics;

var worker = new IrrigationWorker();
await worker.RunTwoCyclesAsync(CancellationToken.None);

internal sealed class IrrigationWorker
{
    public async Task RunTwoCyclesAsync(CancellationToken shutdown)
    {
        for (var cycle = 1; cycle <= 2; cycle++)
        {
            var timer = Stopwatch.StartNew();
            using var budget = CancellationTokenSource.CreateLinkedTokenSource(shutdown);
            budget.CancelAfter(TimeSpan.FromMilliseconds(120));
            try
            {
                await SimulateValveAsync(budget.Token);
                Console.WriteLine($"cycle={cycle} status=ok elapsedMs={timer.ElapsedMilliseconds}");
            }
            catch (OperationCanceledException) when (budget.IsCancellationRequested)
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
