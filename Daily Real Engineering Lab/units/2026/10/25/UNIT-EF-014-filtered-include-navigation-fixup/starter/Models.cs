using Microsoft.EntityFrameworkCore;

public sealed class Cohort
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Enrollment> Enrollments { get; set; } = new();
}

public sealed class Enrollment
{
    public int Id { get; set; }
    public int CohortId { get; set; }
    public bool IsActive { get; set; }
    public Cohort Cohort { get; set; } = null!;
}

public sealed class LabDb(DbContextOptions<LabDb> options) : DbContext(options)
{
    public DbSet<Cohort> Cohorts => Set<Cohort>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cohort>().HasKey(x => x.Id);
        modelBuilder.Entity<Enrollment>().HasKey(x => x.Id);
        modelBuilder.Entity<Enrollment>()
            .HasOne(x => x.Cohort)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.CohortId);
    }
}
