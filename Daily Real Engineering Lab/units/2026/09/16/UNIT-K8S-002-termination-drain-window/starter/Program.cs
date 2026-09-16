var pod = new PodSimulator();

await pod.SendAsync("R1");
pod.BeginTermination();
await pod.SendAsync("R2");
pod.StopApplication();
await pod.SendAsync("R3");

Console.WriteLine($"SUMMARY success={pod.SuccessCount} failed={pod.FailedCount} rejected={pod.RejectedCount}");

sealed class PodSimulator
{
    private bool _ready = true;
    private bool _applicationRunning = true;

    public int SuccessCount { get; private set; }
    public int FailedCount { get; private set; }
    public int RejectedCount { get; private set; }

    public void BeginTermination()
    {
        Console.WriteLine("LIFECYCLE termination-started");
        // Investigation note: which signal should change before new traffic
        // is allowed to depend on an instance that is leaving service?
    }

    public void StopApplication()
    {
        _applicationRunning = false;
        _ready = false;
        Console.WriteLine("LIFECYCLE application-stopped");
    }

    public async Task SendAsync(string requestId)
    {
        Console.WriteLine($"REQUEST {requestId} admission ready={_ready}");
        if (!_ready)
        {
            RejectedCount++;
            Console.WriteLine($"REQUEST {requestId} REJECTED");
            return;
        }

        await Task.Delay(20);
        if (_applicationRunning)
        {
            SuccessCount++;
            Console.WriteLine($"REQUEST {requestId} 200");
        }
        else
        {
            FailedCount++;
            Console.WriteLine($"REQUEST {requestId} 503");
        }
    }
}