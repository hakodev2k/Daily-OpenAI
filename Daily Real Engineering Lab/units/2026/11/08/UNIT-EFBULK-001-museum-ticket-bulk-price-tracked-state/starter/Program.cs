using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var mode = args.Length == 1 ? args[0] : "run";
if (mode is not ("run" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [run|reproduce|verify]");
    return 2;
}
using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();
var options = new DbContextOptionsBuilder<TicketDb>().UseSqlite(connection).Options;
await using (var seed = new TicketDb(options))
{
    await seed.Database.EnsureCreatedAsync();
    seed.Tickets.AddRange(new Ticket { Id = 1, Label = "Adult", Price = 100m },
        new Ticket { Id = 2, Label = "Child", Price = 50m });
    await seed.SaveChangesAsync();
}
await using var db = new TicketDb(options);
var other = await db.Tickets.SingleAsync(x => x.Id == 2);
other.Label = "Child - pending operator correction";
var result = await new TicketPricingService(db).ApplyHolidayPriceAsync(1, 125m);
await using var independent = new TicketDb(options);
var persisted = await independent.Tickets.AsNoTracking().SingleAsync(x => x.Id == 1);
var pendingPreserved = other.Label == "Child - pending operator correction"
    && db.Entry(other).Property(x => x.Label).IsModified;
Console.WriteLine($"Updated={result.Updated}; Response={result.ConfirmedPrice}; DB={persisted.Price}; PendingPreserved={pendingPreserved}");
var symptom = result.Updated == 1 && result.ConfirmedPrice == 100m && persisted.Price == 125m;
var correct = result.Updated == 1 && result.ConfirmedPrice == 125m && persisted.Price == 125m && pendingPreserved;
if (mode == "run") return 0;
if (mode == "reproduce") return symptom ? 0 : 1;
return correct ? 0 : 1;
