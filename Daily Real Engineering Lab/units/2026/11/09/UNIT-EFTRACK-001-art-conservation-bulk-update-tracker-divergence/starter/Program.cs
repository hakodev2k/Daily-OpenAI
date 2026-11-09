using Microsoft.EntityFrameworkCore;

var mode = args.FirstOrDefault() ?? "reproduce";
var options = new DbContextOptionsBuilder<ConservationDb>()
    .UseSqlite("Data Source=:memory:").Options;
await using var db = new ConservationDb(options);
await db.Database.OpenConnectionAsync();
await db.Database.EnsureCreatedAsync();
db.Artifacts.Add(new Artifact { Id = 1, Priority = 1, Note = "initial" });
await db.SaveChangesAsync();

var artifact = await db.Artifacts.SingleAsync(x => x.Id == 1);
var affected = await db.Artifacts.Where(x => x.Id == 1)
    .ExecuteUpdateAsync(s => s.SetProperty(x => x.Priority, 5));
var afterBulk = await db.Artifacts.AsNoTracking().SingleAsync(x => x.Id == 1);
Console.WriteLine($"bulkAffected={affected}; dbAfterBulk={afterBulk.Priority}; tracked={artifact.Priority}");

artifact.Note = "Reviewed by conservator";
// Legacy synchronization code also marks the whole snapshot as modified.
db.Entry(artifact).State = EntityState.Modified;
await db.SaveChangesAsync();

var final = await db.Artifacts.AsNoTracking().SingleAsync(x => x.Id == 1);
Console.WriteLine($"finalPriority={final.Priority}; finalNote={final.Note}");
if (mode == "verify")
{
    if (affected != 1 || afterBulk.Priority != 5 || final.Priority != 5 || final.Note != "Reviewed by conservator")
        throw new Exception("FAIL: bulk priority must survive subsequent note edit.");
    Console.WriteLine("PASS: priority and note preserved.");
}
else if (mode != "reproduce") throw new ArgumentException("Use reproduce or verify.");

public sealed class Artifact
{
    public int Id { get; set; }
    public int Priority { get; set; }
    public string Note { get; set; } = "";
}
public sealed class ConservationDb(DbContextOptions<ConservationDb> options) : DbContext(options)
{
    public DbSet<Artifact> Artifacts => Set<Artifact>();
}
