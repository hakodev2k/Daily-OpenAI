var events = new List<string>();
var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

async Task HandleAsync(int id)
{
    events.Add($"item-{id}-started");
    await gate.Task;
    if (id == 2) throw new InvalidOperationException("notification failed");
    events.Add($"item-{id}-completed");
}

Task ProcessBatchAsync()
{
    foreach (var id in new[] { 1, 2 })
    {
        _ = HandleAsync(id);
    }

    events.Add("batch-completed");
    return Task.CompletedTask;
}

await ProcessBatchAsync();
var completedBeforeRelease = events.Contains("batch-completed") && !events.Contains("item-1-completed");
gate.SetResult();
await Task.Delay(50);

Console.WriteLine(string.Join(Environment.NewLine, events));
Console.WriteLine($"completed-before-required-work={completedBeforeRelease}");

if (args.Contains("--reproduce"))
    return completedBeforeRelease ? 0 : 2;

if (args.Contains("--verify"))
{
    var batchSucceeded = events.Contains("batch-completed");
    return !completedBeforeRelease && !batchSucceeded ? 0 : 3;
}

return 0;