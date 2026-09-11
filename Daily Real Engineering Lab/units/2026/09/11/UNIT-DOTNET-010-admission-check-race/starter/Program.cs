var mode = args.FirstOrDefault() ?? "run";
using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));

var observer = new RendezvousObserver(expectedParticipants: 2, releaseAfter: TimeSpan.FromMilliseconds(250));
var gate = new AdmissionGate(capacity: 1, observer);

async Task WorkAsync(CancellationToken cancellationToken)
{
    await Task.Delay(120, cancellationToken);
}

var first = gate.TryRunAsync("export-A", WorkAsync, timeout.Token);
await Task.Delay(20, timeout.Token);
var second = gate.TryRunAsync("export-B", WorkAsync, timeout.Token);

var results = await Task.WhenAll(first, second);
foreach (var result in results)
{
    Console.WriteLine($"{result.RequestId}: {(result.Accepted ? "Accepted" : "Rejected")}, admission-wait={result.WaitMilliseconds}ms");
}

var accepted = results.Count(x => x.Accepted);
var rejected = results.Count(x => !x.Accepted);
Console.WriteLine($"accepted={accepted}; rejected={rejected}");

if (mode.Equals("reproduce", StringComparison.OrdinalIgnoreCase))
{
    if (accepted == 2)
    {
        Console.WriteLine("REPRODUCED: single-slot policy accepted both callers.");
        return 0;
    }

    Console.Error.WriteLine("Expected the original starter symptom: both callers accepted.");
    return 1;
}

if (mode.Equals("verify", StringComparison.OrdinalIgnoreCase))
{
    if (accepted == 1 && rejected == 1)
    {
        Console.WriteLine("VERIFIED: one caller accepted and one rejected at the admission boundary.");
        return 0;
    }

    Console.Error.WriteLine("Verification failed: capacity=1 must accept exactly one caller.");
    return 1;
}

return 0;

sealed class RendezvousObserver : IAdmissionObserver
{
    private readonly int _expectedParticipants;
    private readonly TimeSpan _releaseAfter;
    private readonly TaskCompletionSource _release = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _arrived;

    public RendezvousObserver(int expectedParticipants, TimeSpan releaseAfter)
    {
        _expectedParticipants = expectedParticipants;
        _releaseAfter = releaseAfter;
    }

    public async Task AfterCapacityObservationAsync(CancellationToken cancellationToken)
    {
        if (Interlocked.Increment(ref _arrived) >= _expectedParticipants)
        {
            _release.TrySetResult();
        }

        var delay = Task.Delay(_releaseAfter, cancellationToken);
        await Task.WhenAny(_release.Task, delay);
    }
}
