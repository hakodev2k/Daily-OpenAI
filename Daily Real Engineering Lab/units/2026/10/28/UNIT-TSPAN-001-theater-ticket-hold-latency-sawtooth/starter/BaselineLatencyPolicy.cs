namespace TicketHoldLab;

// Fixture bất biến để reproduce hành vi của bản phát hành ban đầu.
// Không sửa file này trong quá trình thực hiện lab.
internal static class BaselineLatencyPolicy
{
    public static bool IsOverBudget(TimeSpan elapsed, TimeSpan budget)
    {
        if (elapsed < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(elapsed));
        if (budget <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(budget));

        return elapsed.Milliseconds > budget.TotalMilliseconds;
    }
}
