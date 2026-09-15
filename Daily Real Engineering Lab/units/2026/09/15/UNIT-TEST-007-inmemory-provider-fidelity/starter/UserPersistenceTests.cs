using Microsoft.EntityFrameworkCore;
using Xunit;

public sealed class UserAccount
{
    public int Id { get; set; }
    public required string NormalizedEmail { get; set; }
}

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<UserAccount> Users => Set<UserAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>()
            .HasIndex(x => x.NormalizedEmail)
            .IsUnique();
    }
}

public sealed class UserPersistenceTests
{
    [Fact]
    public async Task Registration_persistence_flow_accepts_two_requests()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase($"identity-{Guid.NewGuid()}")
            .Options;

        await using var db = new IdentityDbContext(options);

        db.Users.Add(new UserAccount { NormalizedEmail = "DEV@EXAMPLE.COM" });
        await db.SaveChangesAsync();

        db.Users.Add(new UserAccount { NormalizedEmail = "DEV@EXAMPLE.COM" });
        await db.SaveChangesAsync();

        Assert.Equal(2, await db.Users.CountAsync());
    }
}