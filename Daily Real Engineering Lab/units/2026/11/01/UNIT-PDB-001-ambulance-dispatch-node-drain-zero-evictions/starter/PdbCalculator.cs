public static class PdbCalculator
{
    // Model used by the maintenance planner before requesting an eviction.
    public static int AllowedDisruptions(int expectedPods, int healthyPods, int minAvailablePercent)
    {
        if (expectedPods < 0 || healthyPods < 0 || healthyPods > expectedPods)
            throw new ArgumentOutOfRangeException(nameof(healthyPods));
        if (minAvailablePercent is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(minAvailablePercent));

        int desiredHealthy = (int)Math.Floor(expectedPods * minAvailablePercent / 100.0);
        return Math.Max(0, healthyPods - desiredHealthy);
    }
}
