using CancellationLab;

internal static class Program
{
    private sealed record Observation(
        string Signal, bool ScannerCalled, bool DownstreamTokenCancelled,
        bool TaskCancelled, bool PendingBeforeRelease);

    private static async Task<int> Main(string[] args)
    {
        var mode = args.Length == 0 ? "observe" : args[0].ToLowerInvariant();
        if (mode is not ("observe" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Usage: observe | reproduce | verify");
            return 2;
        }

        try
        {
            var caller = await ObserveCancellationAsync(stopHost: false);
            var host = await ObserveCancellationAsync(stopHost: true);
            Print(caller);
            Print(host);

            if (mode == "reproduce")
            {
                var reproduced = new[] { caller, host }.All(x =>
                    x.ScannerCalled && !x.DownstreamTokenCancelled &&
                    !x.TaskCancelled && x.PendingBeforeRelease);
                if (!reproduced)
                    throw new InvalidOperationException("Starter symptom not reproduced. Check whether starter was already modified.");
                Console.WriteLine("EXPECTED_REPRODUCED");
                return 0;
            }

            if (mode == "verify")
            {
                foreach (var x in new[] { caller, host })
                {
                    if (!x.ScannerCalled || !x.DownstreamTokenCancelled ||
                        !x.TaskCancelled || x.PendingBeforeRelease)
                        throw new InvalidOperationException(
                            $"Cancellation contract failed for {x.Signal}.");
                }

                await VerifyNormalCompletionAsync();
                Console.WriteLine("VERIFIED");
                return 0;
            }

            Console.WriteLine("OBSERVED (no assertions)");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FAILED: {ex.GetType().Name}: {ex.Message}");
            return 1;
        }
    }

    private static void Print(Observation x) =>
        Console.WriteLine(
            $"{x.Signal}: scannerCalled={x.ScannerCalled}; " +
            $"downstreamTokenCancelled={x.DownstreamTokenCancelled}; " +
            $"taskCancelled={x.TaskCancelled}; pendingBeforeRelease={x.PendingBeforeRelease}");

    private static async Task<Observation> ObserveCancellationAsync(bool stopHost)
    {
        using var caller = new CancellationTokenSource();
        using var host = new CancellationTokenSource();
        var scanner = new ControlledScanner();
        var service = new DocumentValidationService(scanner);
        var pending = service.ValidateAsync(
            new ScanRequest("claim-1042", "sha256-demo"),
            caller.Token, host.Token);

        if (!scanner.WasCalled)
            throw new InvalidOperationException("Scanner was not invoked.");

        if (stopHost) host.Cancel();
        else caller.Cancel();

        bool taskCancelled = false;
        try
        {
            // Bounded wait is a guard against a hung exercise, not a performance benchmark.
            await pending.WaitAsync(TimeSpan.FromMilliseconds(350));
        }
        catch (OperationCanceledException)
        {
            taskCancelled = true;
        }
        catch (TimeoutException)
        {
            // The operation remained pending after cancellation.
        }
        finally
        {
            var tokenCancelled = scanner.LastToken.IsCancellationRequested;
            var stillPending = !pending.IsCompleted;
            scanner.Release();
            try { await pending.WaitAsync(TimeSpan.FromSeconds(2)); }
            catch (OperationCanceledException) { }
            // Preserve the observation from before the forced release.
            scanner.LastObservation = new ObservationState(tokenCancelled, stillPending);
        }

        return new Observation(
            stopHost ? "host" : "caller", scanner.WasCalled,
            scanner.LastObservation!.TokenCancelled, taskCancelled,
            scanner.LastObservation.Pending);
    }

    private static async Task VerifyNormalCompletionAsync()
    {
        using var caller = new CancellationTokenSource();
        using var host = new CancellationTokenSource();
        var scanner = new ControlledScanner();
        var service = new DocumentValidationService(scanner);
        var pending = service.ValidateAsync(
            new ScanRequest("claim-normal", "sha256-normal"),
            caller.Token, host.Token);
        if (!scanner.WasCalled || pending.IsCompleted)
            throw new InvalidOperationException("Normal scan must be pending until the scanner responds.");
        scanner.Release();
        var receipt = await pending.WaitAsync(TimeSpan.FromSeconds(2));
        if (receipt.DocumentId != "claim-normal" || receipt.Verdict != "CLEAN")
            throw new InvalidOperationException("Normal completion returned an invalid receipt.");
    }

    private sealed record ObservationState(bool TokenCancelled, bool Pending);

    private sealed class ControlledScanner : IExternalScanner
    {
        private readonly TaskCompletionSource<ScanReceipt> _completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        private string _documentId = "";
        public bool WasCalled { get; private set; }
        public CancellationToken LastToken { get; private set; }
        public ObservationState? LastObservation { get; set; }

        public Task<ScanReceipt> ScanAsync(
            ScanRequest request, CancellationToken cancellationToken)
        {
            WasCalled = true;
            LastToken = cancellationToken;
            _documentId = request.DocumentId;
            return _completion.Task.WaitAsync(cancellationToken);
        }

        public void Release() =>
            _completion.TrySetResult(new ScanReceipt(_documentId, "CLEAN"));
    }
}
