using BikeShare.Lab;

return await LabHarness.RunAsync(args);

internal static class LabHarness
{
    public static Task<int> RunAsync(string[] args)
    {
        var mode = args.Length == 0 ? "run" : args[0].ToLowerInvariant();
        if (mode is not ("run" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run -- [run|reproduce|verify]");
            return Task.FromResult(2);
        }

        // Always place the simulated pass in the future relative to the real clock.
        // The harness never sleeps and never modifies the OS clock.
        var baseline = DateTimeOffset.UtcNow.AddDays(2);
        var clock = new ManualClock(baseline);
        var gate = new PassGate(clock);
        var pass = new UnlockPass("BIKE-42", baseline.AddMinutes(-5), baseline.AddMinutes(15));

        var start = gate.Evaluate(pass);
        clock.SetUtcNow(baseline.AddMinutes(14).AddSeconds(59));
        var justBeforeExpiry = gate.Evaluate(pass);
        clock.SetUtcNow(baseline.AddMinutes(15));
        var atExpiry = gate.Evaluate(pass);
        clock.SetUtcNow(baseline.AddMinutes(-5).AddTicks(-1));
        var beforeStart = gate.Evaluate(pass);
        var invalid = gate.Evaluate(new UnlockPass("BAD", baseline, baseline));
        clock.SetUtcNow(baseline);
        var afterRewind = gate.Evaluate(pass);

        Console.WriteLine($"SimulatedNowUtc={gate.DiagnosticNowUtc():O}");
        Console.WriteLine($"atStart={start}; beforeExpiry={justBeforeExpiry}; atExpiry={atExpiry}");
        Console.WriteLine($"beforeStart={beforeStart}; invalidWindow={invalid}; afterRewind={afterRewind}");

        var originalSymptom =
            start == GateDecision.NotYetValid &&
            justBeforeExpiry == GateDecision.NotYetValid &&
            atExpiry == GateDecision.NotYetValid &&
            beforeStart == GateDecision.NotYetValid &&
            invalid == GateDecision.InvalidWindow &&
            afterRewind == GateDecision.NotYetValid;

        var fixedContract =
            start == GateDecision.Allowed &&
            justBeforeExpiry == GateDecision.Allowed &&
            atExpiry == GateDecision.Expired &&
            beforeStart == GateDecision.NotYetValid &&
            invalid == GateDecision.InvalidWindow &&
            afterRewind == GateDecision.Allowed;

        if (mode == "run")
        {
            Console.WriteLine("Observation only; run reproduce or verify for assertions.");
            return Task.FromResult(0);
        }

        if (mode == "reproduce")
        {
            Console.WriteLine(originalSymptom ? "REPRODUCED: clock-contract mismatch." : "NOT REPRODUCED: inspect starter and scenario.");
            return Task.FromResult(originalSymptom ? 0 : 1);
        }

        Console.WriteLine(fixedContract ? "PASS: temporal contract at all boundaries." : "FAIL: temporal contract not satisfied.");
        return Task.FromResult(fixedContract ? 0 : 1);
    }
}

internal sealed class ManualClock : TimeProvider
{
    private DateTimeOffset _now;
    public ManualClock(DateTimeOffset initialUtc) => _now = initialUtc;
    public override DateTimeOffset GetUtcNow() => _now;
    public void SetUtcNow(DateTimeOffset now) => _now = now;
}
