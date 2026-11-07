// Reference change for starter HandleAsync:
// keep the delivery lock valid while required processing is active.
// In a real Azure Service Bus application prefer the SDK processor's bounded auto-lock-renewal
// or an explicit renewal loop plus a durable idempotency boundary.

async Task HandleWithRenewalAsync(Delivery d)
{
    using var stop = new CancellationTokenSource();
    var renewal = Task.Run(async () =>
    {
        while (!stop.IsCancellationRequested)
        {
            await Task.Delay(60, stop.Token);
            await broker.RenewAsync(d);
        }
    });

    try
    {
        await Task.Delay(d.Item.Duration);
        effects.TryAdd(d.Item.Id, 1); // simulation only; production idempotency must be durable
        await broker.CompleteAsync(d);
    }
    finally
    {
        stop.Cancel();
        try { await renewal; } catch (OperationCanceledException) { }
    }
}