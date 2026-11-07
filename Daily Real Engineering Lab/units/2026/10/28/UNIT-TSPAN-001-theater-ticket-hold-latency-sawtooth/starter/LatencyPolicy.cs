namespace TicketHoldLab;

public static class LatencyPolicy
{
    // Đầu vào là duration đã được đo bằng đồng hồ monotonic ở lớp gọi.
    // Chỉ trả true khi elapsed vượt budget (không tính trường hợp bằng nhau).
    public static bool IsOverBudget(TimeSpan elapsed, TimeSpan budget)
    {
        if (elapsed < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(elapsed));
        if (budget <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(budget));

        return elapsed.Milliseconds > budget.TotalMilliseconds;
    }
}
