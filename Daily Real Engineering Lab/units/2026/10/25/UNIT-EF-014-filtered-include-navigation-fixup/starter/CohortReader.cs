using Microsoft.EntityFrameworkCore;

public static class CohortReader
{
    // Investigation: hãy so sánh kết quả của cùng query ở context mới
    // với context đã thực hiện một truy vấn khác trong request.
    public static async Task<int[]> GetActiveEnrollmentIdsAsync(
        LabDb db, int cohortId, CancellationToken cancellationToken = default)
    {
        var cohort = await db.Cohorts
            .Include(c => c.Enrollments.Where(e => e.IsActive))
            .SingleAsync(c => c.Id == cohortId, cancellationToken);

        return cohort.Enrollments.Select(e => e.Id).OrderBy(id => id).ToArray();
    }
}
