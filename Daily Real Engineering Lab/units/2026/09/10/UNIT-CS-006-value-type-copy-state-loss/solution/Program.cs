var counters = new Dictionary<string, RateCounter>
{
    ["tenant-a"] = new RateCounter(0)
};

for (var iteration = 1; iteration <= 3; iteration++)
{
    if (!counters.TryGetValue("tenant-a", out var counter))
    {
        throw new InvalidOperationException("Counter not found.");
    }

    counter.Increment();
    counters["tenant-a"] = counter;

    Console.WriteLine($"Iteration={iteration} ObservedCount={counters["tenant-a"].Count}");
}

Console.WriteLine($"PersistedCount={counters["tenant-a"].Count}");

public struct RateCounter
{
    public RateCounter(int count)
    {
        Count = count;
    }

    public int Count { get; private set; }

    public void Increment()
    {
        Count++;
    }
}
