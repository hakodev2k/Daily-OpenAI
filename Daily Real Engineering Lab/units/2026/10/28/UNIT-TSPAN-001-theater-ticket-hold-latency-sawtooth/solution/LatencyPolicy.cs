namespace TicketHoldLab;

public static class LatencyPolicy
{
    public static bool IsOverBudget(TimeSpan elapsed, TimeSpan budget)
    {
        if (elapsed < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(elapsed));
        if (budget <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(budget));

        return elapsed > budget;
    }
}
