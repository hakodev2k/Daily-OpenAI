using System.Linq;

if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- reproduce|verify");
    return 2;
}

return args[0] == "reproduce" ? await ReproduceAsync() : await VerifyAsync();

static async Task<int> ReproduceAsync()
{
    var pipeline = new AuditPipeline();
    for (int i = 1; i <= 12; i++)
        await pipeline.PublishAsync(new AuditEvent(i, "role-change"));
    pipeline.Complete();

    var observed = new List<int>();
    await foreach (var item in pipeline.ReadAllAsync())
        observed.Add(item.Sequence);

    Console.WriteLine("published=12; observed=" + observed.Count +
                      "; sequences=[" + string.Join(",", observed) + "]");
    // Reproduce succeeds only if the ORIGINAL symptom was actually observed.
    bool symptom = observed.Count < 12 &&
                   observed.SequenceEqual(Enumerable.Range(9, 4));
    Console.WriteLine(symptom ? "REPRODUCED: missing audit events" :
                                "NOT REPRODUCED: inspect starter state");
    return symptom ? 0 : 1;
}

static async Task<int> VerifyAsync()
{
    var pipeline = new AuditPipeline();
    var startConsumer = new TaskCompletionSource<bool>(
        TaskCreationOptions.RunContinuationsAsynchronously);
    var observed = new List<int>();

    var consumer = Task.Run(async () =>
    {
        await startConsumer.Task;
        await foreach (var item in pipeline.ReadAllAsync())
            observed.Add(item.Sequence);
    });

    var producer = Task.Run(async () =>
    {
        for (int i = 1; i <= 12; i++)
            await pipeline.PublishAsync(new AuditEvent(i, "role-change"));
        pipeline.Complete();
    });

    // While the consumer is gated, a bounded lossless pipeline must apply
    // pressure to its producer instead of completing all 12 publishes.
    await Task.Delay(200);
    bool boundedPressureObserved = !producer.IsCompleted;
    startConsumer.TrySetResult(true);

    try
    {
        await Task.WhenAll(producer, consumer).WaitAsync(TimeSpan.FromSeconds(5));
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("VERIFY FAILED: pipeline did not drain: " + ex.Message);
        return 1;
    }

    bool completeOrdered = observed.SequenceEqual(Enumerable.Range(1, 12));
    Console.WriteLine("published=12; observed=" + observed.Count +
                      "; sequences=[" + string.Join(",", observed) + "]");
    Console.WriteLine("boundedPressureObserved=" + boundedPressureObserved);
    Console.WriteLine("completeOrdered=" + completeOrdered);
    bool ok = boundedPressureObserved && completeOrdered;
    Console.WriteLine(ok ? "VERIFY PASSED" : "VERIFY FAILED");
    return ok ? 0 : 1;
}
