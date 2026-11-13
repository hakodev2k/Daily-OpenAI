using System.Globalization;
namespace TicketGateway;
public sealed record GatewayReply(int Status, string? RetryAfter);
public static class RetryPolicy
{
    // Dùng thời gian từ server để quyết định khi nào thử lại.
    public static TimeSpan Delay(GatewayReply reply, DateTimeOffset now)
    {
        if (reply.Status != 429 && reply.Status != 503) return TimeSpan.Zero;
        if (string.IsNullOrWhiteSpace(reply.RetryAfter)) return TimeSpan.FromSeconds(1);
        if (int.TryParse(reply.RetryAfter, NumberStyles.Integer, CultureInfo.InvariantCulture, out int seconds))
            return TimeSpan.FromSeconds(Math.Clamp(seconds, 0, 120));
        // Partner gateway recently changed to send HTTP-date as well.
        return TimeSpan.FromSeconds(1);
    }
}
