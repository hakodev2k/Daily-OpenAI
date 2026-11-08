using System;
using System.Collections.Generic;
using System.Linq;

public record Review(string ApplicantId, string RevisionId, int Score, DateTimeOffset SubmittedAt);

public static class Ranking
{
    public static string[] SelectTop(IEnumerable<Review> reviews, int limit)
    {
        return reviews
            .DistinctBy(r => r.ApplicantId)
            .OrderByDescending(r => r.Score)
            .ThenBy(r => r.ApplicantId, StringComparer.Ordinal)
            .Take(limit)
            .Select(r => r.ApplicantId + ":" + r.RevisionId)
            .ToArray();
    }
}

public static class Program
{
    private static readonly Review[] Input =
    {
        new("AN", "AN-v1", 65, DateTimeOffset.Parse("2026-10-01T09:00:00+07:00")),
        new("BINH", "BINH-v1", 88, DateTimeOffset.Parse("2026-10-01T09:01:00+07:00")),
        new("AN", "AN-v2", 99, DateTimeOffset.Parse("2026-10-02T09:00:00+07:00")),
        new("CHI", "CHI-v1", 90, DateTimeOffset.Parse("2026-10-01T09:02:00+07:00")),
        new("CHI", "CHI-v2", 90, DateTimeOffset.Parse("2026-10-02T09:02:00+07:00")),
    };
    private static readonly string[] Expected = { "AN:AN-v2", "CHI:CHI-v2" };

    public static int Main(string[] args)
    {
        var a = Ranking.SelectTop(Input, 2);
        var b = Ranking.SelectTop(Input.Reverse(), 2);
        Console.WriteLine("Original: " + string.Join(", ", a));
        Console.WriteLine("Reversed: " + string.Join(", ", b));
        bool pass = a.SequenceEqual(Expected) && b.SequenceEqual(Expected);
        if (args.Contains("--verify"))
        {
            // Probe each possible cyclic rotation and the reverse permutation.
            for (int i = 0; i < Input.Length; i++)
            {
                var rotated = Input.Skip(i).Concat(Input.Take(i));
                pass &= Ranking.SelectTop(rotated, 2).SequenceEqual(Expected);
                pass &= Ranking.SelectTop(rotated.Reverse(), 2).SequenceEqual(Expected);
            }
            pass &= Ranking.SelectTop(Array.Empty<Review>(), 2).Length == 0;
            pass &= Ranking.SelectTop(Input, 0).Length == 0;
        }
        Console.WriteLine(pass ? "PASS: ranking contract" : "FAIL: ranking contract");
        return pass ? 0 : 1;
    }
}
