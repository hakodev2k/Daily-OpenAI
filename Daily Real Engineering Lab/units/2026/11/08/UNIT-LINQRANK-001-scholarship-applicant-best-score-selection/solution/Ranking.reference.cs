using System;
using System.Collections.Generic;
using System.Linq;

public record Review(string ApplicantId, string RevisionId, int Score, DateTimeOffset SubmittedAt);

public static class Ranking
{
    public static string[] SelectTop(IEnumerable<Review> reviews, int limit)
    {
        return reviews
            .OrderByDescending(r => r.Score)
            .ThenByDescending(r => r.SubmittedAt)
            .ThenBy(r => r.RevisionId, StringComparer.Ordinal)
            .DistinctBy(r => r.ApplicantId)
            .OrderByDescending(r => r.Score)
            .ThenBy(r => r.ApplicantId, StringComparer.Ordinal)
            .Take(limit)
            .Select(r => r.ApplicantId + ":" + r.RevisionId)
            .ToArray();
    }
}
