using LibraryPickup;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var mode = args.Length == 1 ? args[0].ToLowerInvariant() : "run";
        if (mode is not ("run" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Cách dùng: dotnet run --project harness -- [run|reproduce|verify]");
            return 2;
        }

        var fixtures = new (string Name, PickupJob[] Jobs, bool Overlap)[]
        {
            ("Hai chi nhánh", new[]
            {
                new PickupJob("REQ-N-101", "NORTH"),
                new PickupJob("REQ-S-202", "SOUTH")
            }, true),
            ("Đảo thứ tự", new[]
            {
                new PickupJob("REQ-W-303", "WEST"),
                new PickupJob("REQ-E-404", "EAST")
            }, true),
            ("Một yêu cầu", new[] { new PickupJob("REQ-ONE-501", "CENTRAL") }, false),
            ("Hàng đợi rỗng", Array.Empty<PickupJob>(), false)
        };

        var allCorrect = true;
        var reproduced = 0;

        foreach (var fixture in fixtures)
        {
            var gate = new TwoWorkerGate();
            var processed = await new PickupDispatcher().DispatchAsync(
                fixture.Jobs,
                fixture.Overlap ? gate.WaitAsync : null);

            var expected = fixture.Jobs
                .Select(x => x.RequestId)
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToArray();
            var actual = processed
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToArray();

            var correct = actual.SequenceEqual(expected, StringComparer.Ordinal);
            allCorrect &= correct;
            var duplicate = actual.Length == 2 && actual[0] == actual[1];
            if (fixture.Overlap && duplicate && !correct)
                reproduced++;

            Console.WriteLine($"[{fixture.Name}] expected={string.Join(",", expected)} actual={string.Join(",", actual)} correct={correct}");
        }

        if (mode == "reproduce")
        {
            var found = reproduced == 2;
            Console.WriteLine(found ? "REPRODUCED: hai fixture có phiếu trùng và yêu cầu bị thiếu." :
                "NOT REPRODUCED: không quan sát đúng triệu chứng gốc.");
            return found ? 0 : 1;
        }

        if (mode == "verify")
        {
            Console.WriteLine(allCorrect ? "VERIFIED: mọi fixture giữ nguyên tập yêu cầu." :
                "FAILED: có phiếu trùng hoặc yêu cầu bị thiếu.");
            return allCorrect ? 0 : 1;
        }

        return 0;
    }

    private sealed class TwoWorkerGate
    {
        private int _arrivals;
        private readonly TaskCompletionSource<bool> _ready =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task WaitAsync()
        {
            if (Interlocked.Increment(ref _arrivals) == 2)
                _ready.TrySetResult(true);

            return _ready.Task.WaitAsync(TimeSpan.FromSeconds(10));
        }
    }
}
