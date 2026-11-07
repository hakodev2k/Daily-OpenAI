var gate = new SemaphoreSlim(1, 1);
var completion = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

var observer = completion.Task.ContinueWith(
    completedTask =>
    {
        Console.WriteLine("observer: waiting for gate");
        gate.Wait();
        try
        {
            Console.WriteLine($"observer: received {completedTask.Result}");
        }
        finally
        {
            gate.Release();
        }
    },
    CancellationToken.None,
    TaskContinuationOptions.ExecuteSynchronously,
    TaskScheduler.Default);

await gate.WaitAsync();
Console.WriteLine("dispatcher: acquired gate");

try
{
    Console.WriteLine("dispatcher: completing signal");
    completion.SetResult("ready");
    Console.WriteLine("dispatcher: signal completed");
}
finally
{
    gate.Release();
    Console.WriteLine("dispatcher: released gate");
}

await observer;
Console.WriteLine("completed");
