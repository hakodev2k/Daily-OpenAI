namespace SloAlertLab;

// Đây là chính sách đang chạy trong hệ thống mô phỏng.
// Người học chỉ cần sửa file này; Program.cs là harness kiểm chứng hành vi.
internal static class AlertPolicy
{
    public static bool ShouldPage(IReadOnlyList<MinuteBucket> minutes)
    {
        ArgumentNullException.ThrowIfNull(minutes);
        // Ngưỡng cũ được đội vận hành cấu hình khi lưu lượng còn thấp.
        return minutes.TakeLast(30).Any(minute => minute.Errors >= 20);
    }
}
