using AuditLab;

var mode = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();
return await LabHarness.RunAsync(mode);

internal static class LabHarness
{
    public static async Task<int> RunAsync(string mode)
    {
        if (mode is not ("demo" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Usage: demo | reproduce | verify");
            return 2;
        }

        try
        {
            var observation = await ObserveDeliveryAsync();
            var cancellationWorks = await ObserveCancellationAsync();
            var completeDelivery = observation.Received.SequenceEqual(new long[] { 1, 2, 3, 4, 5 });
            Console.WriteLine($"published=5 received={observation.Received.Count}");
            Console.WriteLine($"receivedSequences=[{string.Join(",", observation.Received)}]");
            Console.WriteLine($"producerCompletedBeforeConsumer={observation.ProducerCompletedBeforeConsumer}");
            Console.WriteLine($"pendingProducerCancelled={cancellationWorks}");
            Console.WriteLine($"completeOrderedDelivery={completeDelivery}");

            if (mode == "demo") return 0;

            if (mode == "reproduce")
            {
                var symptomConfirmed = !completeDelivery
                    && observation.ProducerCompletedBeforeConsumer
                    && !cancellationWorks;
                Console.WriteLine(symptomConfirmed ? "REPRODUCED: audit delivery contract broken" : "NOT REPRODUCED");
                return symptomConfirmed ? 0 : 1;
            }

            var verified = completeDelivery
                && !observation.ProducerCompletedBeforeConsumer
                && cancellationWorks;
            Console.WriteLine(verified ? "VERIFIED: delivery, backpressure and cancellation" : "VERIFY FAILED");
            return verified ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            return 2;
        }
    }

    private static async Task<DeliveryObservation> ObserveDeliveryAsync()
    {
        var pipeline = new AuditPipeline(capacity: 2);
        var records = Enumerable.Range(1, 5)
            .Select(n => new AuditRecord(n, "BATCH-SYNTHETIC-42", "approval-step"))
            .ToArray();

        // Publisher được gọi trước consumer để mô phỏng một burst đã đến.
        var publishTask = pipeline.PublishAsync(records, CancellationToken.None);
        var producerCompletedBeforeConsumer = publishTask.IsCompleted;
        var consumeTask = pipeline.ReadAllAsync(CancellationToken.None);

        await publishTask.WaitAsync(TimeSpan.FromSeconds(5));
        pipeline.Complete();
        var received = await consumeTask.WaitAsync(TimeSpan.FromSeconds(5));
        return new DeliveryObservation(received.Select(x => x.Sequence).ToArray(),
            producerCompletedBeforeConsumer);
    }

    private static async Task<bool> ObserveCancellationAsync()
    {
        var pipeline = new AuditPipeline(capacity: 2);
        using var cts = new CancellationTokenSource();
        var records = Enumerable.Range(1, 3)
            .Select(n => new AuditRecord(n, "BATCH-SYNTHETIC-43", "release-check"))
            .ToArray();

        // Không khởi động consumer: kiểm tra tác vụ thứ ba khi capacity đã hết.
        var pending = pipeline.PublishAsync(records, cts.Token);
        var wasWaiting = !pending.IsCompleted;
        cts.Cancel();

        try
        {
            await pending.WaitAsync(TimeSpan.FromSeconds(5));
            return false;
        }
        catch (OperationCanceledException)
        {
            return wasWaiting;
        }
        finally
        {
            pipeline.Complete();
        }
    }

    private sealed record DeliveryObservation(long[] Received, bool ProducerCompletedBeforeConsumer);
}
