var events = new List<string>();
var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

async Task HandleAsync(int id)
{
    events.Add($"item-{id}-started");
    await gate.Task;
    if (id == 2) throw new InvalidOperationException("notification failed");
    events.Add($"item-{id}-completed");
}

async Task ProcessBatchAsync()
{
    var tasks = new[] { 1, 2 }.Select(HandleAsync).ToArray();
    await Task.WhenAll(tasks);
    events.Add("batch-completed");
}

var batchTask = ProcessBatchAsync();
await Task.Delay(20);
var completedBeforeRelease = batchTask.IsCompleted;
gate.SetResult();
var failureObserved = false;
try { await batchTask; }
catch (InvalidOperationException) { failureObserved = true; }

Console.WriteLine($"completed-before-required-work={completedBeforeRelease}");
Console.WriteLine($"failure-observed-by-batch={failureObserved}");
return !completedBeforeRelease && failureObserved && !events.Contains("batch-completed") ? 0 : 1;