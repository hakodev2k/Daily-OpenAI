using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GradesLab;

public sealed class GradeRecord
{
    public int Id { get; set; }
    public int Score { get; set; }
    public string Status { get; set; } = "";
    public string ReviewerNote { get; set; } = "";
}

public sealed class GradesDbContext(DbContextOptions<GradesDbContext> options) : DbContext(options)
{
    public DbSet<GradeRecord> Grades => Set<GradeRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GradeRecord>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.ReviewerNote).IsRequired();
        });
    }
}

public static class GradeWorkflow
{
    public static async Task<GradeRecord> RunAsync()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<GradesDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new GradesDbContext(options);
        await db.Database.EnsureCreatedAsync();

        db.Grades.Add(new GradeRecord
        {
            Id = 41, Score = 92, Status = "Draft", ReviewerNote = ""
        });
        await db.SaveChangesAsync();

        // Simulate an existing request scope with an entity already loaded.
        db.ChangeTracker.Clear();
        var grade = await db.Grades.SingleAsync(x => x.Id == 41);

        // A separate batch operation publishes eligible results.
        int changed = await db.Grades.Where(x => x.Id == 41 && x.Score >= 60)
            .ExecuteUpdateAsync(updates => updates
                .SetProperty(x => x.Status, "Published"));
        if (changed != 1) throw new InvalidOperationException("Unexpected publish count");

        // A reviewer then amends a note in the same request scope.
        grade.ReviewerNote = "QA-reviewed";
        db.Grades.Update(grade);
        await db.SaveChangesAsync();

        // Read from storage, not from the previously tracked instance.
        db.ChangeTracker.Clear();
        return await db.Grades.AsNoTracking().SingleAsync(x => x.Id == 41);
    }
}
