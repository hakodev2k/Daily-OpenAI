using System.IO.Pipelines;
using System.Text;
using ConveyorTelemetry;

string mode = args.Length == 1 ? args[0] : "run";
try
{
    return mode switch
    {
        "run" or "reproduce" => await ReproduceAsync(),
        "verify" => await VerifyAsync(),
        _ => Usage()
    };
}
catch (Exception ex)
{
    Console.Error.WriteLine($"UNEXPECTED: {ex}");
    return 2;
}

static int Usage()
{
    Console.Error.WriteLine("Usage: dotnet run -- [run|reproduce|verify]");
    return 2;
}

static async Task<(Pipe Pipe, FrameProbe Probe, Task<string> Read)> StartAsync()
{
    var pipe = new Pipe();
    var probe = new FrameProbe();
    await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("CONVEYOR|4"));
    Task<string> read = new FrameReader().ReadFrameAsync(pipe.Reader, probe.MarkIncomplete);
    await probe.FirstIncomplete.WaitAsync(TimeSpan.FromSeconds(3));
    return (pipe, probe, read);
}

static async Task<int> ReproduceAsync()
{
    var (_, probe, read) = await StartAsync();
    if (await Task.WhenAny(read, Task.Delay(500)) != read)
    {
        Console.Error.WriteLine($"NOT REPRODUCED: reader waited; incompleteObservations={probe.Count}");
        return 1;
    }
    try
    {
        Console.Error.WriteLine($"NOT REPRODUCED: frame={await read}");
        return 1;
    }
    catch (InvalidOperationException ex) when (ex.Message.StartsWith("READ_SPIN_GUARD:", StringComparison.Ordinal))
    {
        Console.WriteLine($"REPRODUCED: {ex.Message}");
        Console.WriteLine("Only one partial fragment was sent by the producer.");
        return 0;
    }
}

static async Task<int> VerifyAsync()
{
    var (pipe, probe, read) = await StartAsync();
    if (await Task.WhenAny(read, Task.Delay(150)) == read)
    {
        try { await read; }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FAILED: reader terminated before new bytes: {ex.Message}");
            return 1;
        }
        Console.Error.WriteLine("FAILED: reader returned before delimiter.");
        return 1;
    }

    await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes("2\n"));
    await pipe.Writer.CompleteAsync();
    string frame;
    try { frame = await read.WaitAsync(TimeSpan.FromSeconds(3)); }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"FAILED: final fragment not processed: {ex.Message}");
        return 1;
    }
    await pipe.Reader.CompleteAsync();
    if (frame != "CONVEYOR|42" || probe.Count != 1)
    {
        Console.Error.WriteLine($"FAILED: frame='{frame}', incompleteObservations={probe.Count}");
        return 1;
    }
    Console.WriteLine("VERIFIED: split frame reconstructed once without busy-reading unchanged bytes.");
    return 0;
}

sealed class FrameProbe
{
    private readonly TaskCompletionSource _first = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _count;
    public Task FirstIncomplete => _first.Task;
    public int Count => Volatile.Read(ref _count);
    public void MarkIncomplete()
    {
        int count = Interlocked.Increment(ref _count);
        if (count == 1) _first.TrySetResult();
        if (count >= 4)
            throw new InvalidOperationException($"READ_SPIN_GUARD: {count} incomplete observations without a producer write");
    }
}
