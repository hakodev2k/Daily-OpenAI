using System.Net.Http;

public static class RetryPlanner
{
    public static TimeSpan GetDelay(
        HttpResponseMessage response, DateTimeOffset now,
        TimeSpan fallback, TimeSpan maxDelay)
    {
        ArgumentNullException.ThrowIfNull(response);
        if (fallback < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(fallback));
        if (maxDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(maxDelay));

        var header = response.Headers.RetryAfter;
        if (header?.Delta is { } delta)
            return Bound(delta, maxDelay);
        if (header?.Date is { } retryAt)
            return Bound(retryAt - now, maxDelay);
        return Bound(fallback, maxDelay);
    }

    private static TimeSpan Bound(TimeSpan value, TimeSpan maxDelay)
        => value <= TimeSpan.Zero ? TimeSpan.Zero
            : value > maxDelay ? maxDelay : value;
}
