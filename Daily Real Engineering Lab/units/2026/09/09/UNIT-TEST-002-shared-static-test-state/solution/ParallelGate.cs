namespace SharedStateLab;

internal static class ParallelGate
{
    private static readonly TaskCompletionSource<bool> StripeReady = NewSignal();
    private static readonly TaskCompletionSource<bool> LegacyReady = NewSignal();

    public static bool Enabled =>
        string.Equals(Environment.GetEnvironmentVariable("LAB_PARALLEL_GATE"), "1", StringComparison.Ordinal);

    public static async Task RendezvousAsync(string participant)
    {
        if (!Enabled) return;

        if (participant == "Stripe")
        {
            StripeReady.TrySetResult(true);
            await LegacyReady.Task.WaitAsync(TimeSpan.FromSeconds(5));
            return;
        }

        LegacyReady.TrySetResult(true);
        await StripeReady.Task.WaitAsync(TimeSpan.FromSeconds(5));
    }

    private static TaskCompletionSource<bool> NewSignal() => new(TaskCreationOptions.RunContinuationsAsynchronously);
}
