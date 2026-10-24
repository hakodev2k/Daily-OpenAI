using Microsoft.EntityFrameworkCore;

public static class CohortReader
{
    public static async Task<int[]> GetActiveEnrollmentIdsAsync(
        LabDb db, int cohortId, CancellationToken cancellationToken = default)
    {
        // Read-only materialization must not merge with the scope's tracked graph.
        var cohort = await db.Cohorts
            .AsNoTracking()
            .Include(c => c.Enrollments.Where(e => e.IsActive))
            .SingleAsync(c => c.Id == cohortId, cancellationToken);

        return cohort.Enrollments.Select(e => e.Id).OrderBy(id => id).ToArray();
    }
}
