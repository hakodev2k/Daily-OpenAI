namespace SloAlertLab;

// Reference Solution — chỉ đọc sau khi tự reproduce và tự sửa.
internal static class AlertPolicy
{
    private const double AllowedErrorRatio = 0.001; // 99.9% availability SLO
    private const double PagingBurn = 14.4;

    public static bool ShouldPage(IReadOnlyList<MinuteBucket> minutes)
    {
        ArgumentNullException.ThrowIfNull(minutes);
        if (minutes.Count < 30) return false; // chưa có đủ bằng chứng cho 30-minute policy

        return BurnRate(minutes, 5) >= PagingBurn
            && BurnRate(minutes, 30) >= PagingBurn;
    }

    private static double BurnRate(IReadOnlyList<MinuteBucket> minutes, int window)
    {
        long requests = 0;
        long errors = 0;
        for (var i = minutes.Count - window; i < minutes.Count; i++)
        {
            var bucket = minutes[i];
            if (bucket.Requests < 0 || bucket.Errors < 0 || bucket.Errors > bucket.Requests)
                throw new ArgumentException("Invalid metric bucket.", nameof(minutes));
            requests += bucket.Requests;
            errors += bucket.Errors;
        }

        return requests == 0 ? 0 : (double)errors / requests / AllowedErrorRatio;
    }
}
