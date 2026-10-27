using System.Threading;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var mode = args.Length == 0 ? "--demo" : args[0];
        if (args.Length > 1 || (mode != "--demo" && mode != "--reproduce" && mode != "--verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run -- [--demo|--reproduce|--verify]");
            return 2;
        }

        var failures = 0;
        var rounds = mode == "--demo" ? 1 : 24;
        for (var round = 1; round <= rounds; round++)
        {
            var two = await RunCaseAsync(new[] { "Lan", "Minh" }, new[] { "Lan", "Minh" });
            var three = await RunCaseAsync(new[] { "Lan", "Minh", "Huy" }, new[] { "Lan", "Minh" });
            var one = await RunCaseAsync(new[] { "Lan" }, new[] { "Lan" });

            if (round == 1)
            {
                Console.WriteLine($"Two on duty: approved={two.Approved}, remaining={two.Remaining}");
                Console.WriteLine($"Three on duty: approved={three.Approved}, remaining={three.Remaining}");
                Console.WriteLine($"One on duty: approved={one.Approved}, remaining={one.Remaining}");
            }

            var baselineSymptom = two == new CaseResult(2, 0);
            var healthyBehavior = two == new CaseResult(1, 1);
            var edgeCases = three == new CaseResult(2, 1) && one == new CaseResult(0, 1);
            var pass = mode switch
            {
                "--reproduce" => baselineSymptom && edgeCases,
                "--verify" => healthyBehavior && edgeCases,
                _ => true
            };
            if (!pass) failures++;
        }

        if (mode == "--demo") return 0;
        Console.WriteLine($"{mode}: {(failures == 0 ? "PASS" : "FAIL")} ({rounds - failures}/{rounds} rounds)");
        return failures == 0 ? 0 : 1;
    }

    private static async Task<CaseResult> RunCaseAsync(string[] roster, string[] leaveRequests)
    {
        var store = new ShiftRosterStore(roster);
        var gate = new TwoPartyGate(leaveRequests.Length);
        var service = new ShiftLeaveService(store, gate);
        var results = await Task.WhenAll(leaveRequests.Select(service.ApproveAsync));
        return new CaseResult(results.Count(approved => approved), store.ActiveCount);
    }

    private readonly record struct CaseResult(int Approved, int Remaining);
}

public sealed class TwoPartyGate
{
    private readonly int _participants;
    private readonly TaskCompletionSource<bool> _release =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _arrivals;

    public TwoPartyGate(int participants)
    {
        if (participants < 1) throw new ArgumentOutOfRangeException(nameof(participants));
        _participants = participants;
    }

    public Task ArriveAsync()
    {
        var count = Interlocked.Increment(ref _arrivals);
        if (count == _participants) _release.TrySetResult(true);
        if (count > _participants) throw new InvalidOperationException("Unexpected participant");
        return _release.Task;
    }
}
