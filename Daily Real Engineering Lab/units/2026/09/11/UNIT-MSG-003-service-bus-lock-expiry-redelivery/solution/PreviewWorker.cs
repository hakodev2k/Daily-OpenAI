sealed class PreviewWorker
{
    public async Task ProcessAsync(Delivery delivery)
    {
        Console.WriteLine($"START message={delivery.MessageId} delivery={delivery.DeliveryNumber}");

        using var stopRenewal = new CancellationTokenSource();
        var renewalTask = RenewUntilStoppedAsync(delivery, stopRenewal.Token);

        try
        {
            await Task.Delay(500);
            delivery.RecordSideEffect();
            Console.WriteLine($"OUTPUT_WRITTEN message={delivery.MessageId} delivery={delivery.DeliveryNumber}");
            delivery.Complete();
            Console.WriteLine($"COMPLETE_ATTEMPT message={delivery.MessageId} delivery={delivery.DeliveryNumber}");
        }
        finally
        {
            stopRenewal.Cancel();
            await renewalTask;
        }
    }

    private static async Task RenewUntilStoppedAsync(Delivery delivery, CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                await Task.Delay(100, cancellationToken);
                delivery.RenewLock();
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }
}
