using Microsoft.Data.Sqlite;
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
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<UserAccount>().HasIndex(x => x.NormalizedEmail).IsUnique();
}

public sealed class UserPersistenceTests
{
    private static async Task<(SqliteConnection Connection, IdentityDbContext Db)> CreateDbAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<IdentityDbContext>().UseSqlite(connection).Options;
        var db = new IdentityDbContext(options);
        await db.Database.EnsureCreatedAsync();
        return (connection, db);
    }

    [Fact]
    public async Task Duplicate_normalized_email_is_rejected_by_persistence_contract()
    {
        var (connection, db) = await CreateDbAsync();
        await using (connection)
        await using (db)
        {
            db.Users.Add(new UserAccount { NormalizedEmail = "DEV@EXAMPLE.COM" });
            await db.SaveChangesAsync();
            db.Users.Add(new UserAccount { NormalizedEmail = "DEV@EXAMPLE.COM" });
            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
    }

    [Fact]
    public async Task Distinct_normalized_emails_are_persisted()
    {
        var (connection, db) = await CreateDbAsync();
        await using (connection)
        await using (db)
        {
            db.Users.AddRange(
                new UserAccount { NormalizedEmail = "A@EXAMPLE.COM" },
                new UserAccount { NormalizedEmail = "B@EXAMPLE.COM" });
            await db.SaveChangesAsync();
            Assert.Equal(2, await db.Users.CountAsync());
        }
    }
}