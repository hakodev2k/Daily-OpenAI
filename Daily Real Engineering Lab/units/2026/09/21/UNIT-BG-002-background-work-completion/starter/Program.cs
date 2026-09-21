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

var batchTask = ProcessBatchAsync();
await Task.Delay(20);
var completedBeforeRelease = batchTask.IsCompleted;
gate.SetResult();
var failureObserved = false;
try { await batchTask; }
catch (InvalidOperationException) { failureObserved = true; }
await Task.Delay(50);

Console.WriteLine(string.Join(Environment.NewLine, events));
Console.WriteLine($"completed-before-required-work={completedBeforeRelease}");
Console.WriteLine($"failure-observed-by-batch={failureObserved}");

if (args.Contains("--reproduce"))
    return completedBeforeRelease && !failureObserved ? 0 : 2;

if (args.Contains("--verify"))
    return !completedBeforeRelease && failureObserved && !events.Contains("batch-completed") ? 0 : 3;

return 0;