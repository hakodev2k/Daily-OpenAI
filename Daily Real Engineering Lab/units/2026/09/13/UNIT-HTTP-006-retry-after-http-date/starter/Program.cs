using System.Globalization;

static TimeSpan GetRetryDelay(string? retryAfter, DateTimeOffset now)
{
    if (int.TryParse(retryAfter, NumberStyles.None, CultureInfo.InvariantCulture, out var seconds))
    {
        return TimeSpan.FromSeconds(Math.Max(0, seconds));
    }

    return TimeSpan.Zero;
}

var now = new DateTimeOffset(2026, 9, 13, 1, 23, 0, TimeSpan.FromHours(7));
var deltaSeconds = "5";
var httpDate = now.AddSeconds(5).ToUniversalTime().ToString("R", CultureInfo.InvariantCulture);

Console.WriteLine($"NOW={now:O}");
Console.WriteLine($"DELTA_HEADER={deltaSeconds}");
Console.WriteLine($"DELTA_DELAY={GetRetryDelay(deltaSeconds, now).TotalSeconds:0}");
Console.WriteLine($"DATE_HEADER={httpDate}");
Console.WriteLine($"DATE_DELAY={GetRetryDelay(httpDate, now).TotalSeconds:0}");