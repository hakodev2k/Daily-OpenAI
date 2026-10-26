using System.Net.Http;

public static class RetryPlanner
{
    // This public contract is shared by the outbound integration.
    public static TimeSpan GetDelay(
        HttpResponseMessage response, DateTimeOffset now,
        TimeSpan fallback, TimeSpan maxDelay)
    {
        ArgumentNullException.ThrowIfNull(response);
        if (fallback < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(fallback));
        if (maxDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(maxDelay));

        var delta = response.Headers.RetryAfter?.Delta;
        if (delta is { } requested)
            return Bound(requested, maxDelay);
        return Bound(fallback, maxDelay);
    }

    private static TimeSpan Bound(TimeSpan value, TimeSpan maxDelay)
        => value <= TimeSpan.Zero ? TimeSpan.Zero
            : value > maxDelay ? maxDelay : value;
}
