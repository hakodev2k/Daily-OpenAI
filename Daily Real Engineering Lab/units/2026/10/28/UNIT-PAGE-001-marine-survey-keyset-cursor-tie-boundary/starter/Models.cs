namespace MarineSurveyLab;

public sealed record Observation(string ObservationId, DateTimeOffset ObservedAtUtc, string Site);
public sealed record Page(IReadOnlyList<Observation> Items, string? NextCursor);

public static class Samples
{
    public static IReadOnlyList<Observation> All { get; } = Build();

    private static IReadOnlyList<Observation> Build()
    {
        var t0 = new DateTimeOffset(2026, 10, 28, 0, 0, 0, TimeSpan.Zero);
        return new[]
        {
            new Observation("obs-d", t0.AddMinutes(1), "reef-east"),
            new Observation("obs-b", t0, "reef-west"),
            new Observation("obs-f", t0.AddMinutes(2), "reef-north"),
            new Observation("obs-a", t0, "reef-west"),
            new Observation("obs-e", t0.AddMinutes(1), "reef-east"),
            new Observation("obs-c", t0, "reef-south")
        };
    }
}
