using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();
var options = new DbContextOptionsBuilder<LibraryDb>().UseSqlite(connection).Options;
using var db = new LibraryDb(options);
db.Database.EnsureCreated();
db.Holds.Add(new Hold { Id = 1, Reader = "An", Status = "Pending" });
db.SaveChanges();

var hold = db.Holds.Single(x => x.Id == 1);
var affected = db.Holds.Where(x => x.Id == 1 && x.Status == "Pending")
    .ExecuteUpdate(setters => setters.SetProperty(x => x.Status, "Approved"));
// A response after a set-based write must explicitly read the persisted state.
var persisted = db.Holds.AsNoTracking().Single(x => x.Id == 1);
Console.WriteLine($"AFFECTED={affected}");
Console.WriteLine($"TRACKED={hold.Status}");
Console.WriteLine($"DATABASE={persisted.Status}");
Console.WriteLine($"RESPONSE={persisted.Status}");

public sealed class Hold
{
    public int Id { get; set; }
    public string Reader { get; set; } = "";
    public string Status { get; set; } = "";
}
public sealed class LibraryDb(DbContextOptions<LibraryDb> options) : DbContext(options)
{
    public DbSet<Hold> Holds => Set<Hold>();
}
