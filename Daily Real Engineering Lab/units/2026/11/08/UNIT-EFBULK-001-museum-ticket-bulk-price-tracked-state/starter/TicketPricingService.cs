using Microsoft.EntityFrameworkCore;

public sealed class Ticket
{
    public int Id { get; set; }
    public string Label { get; set; } = "";
    public decimal Price { get; set; }
}
public sealed class TicketDb(DbContextOptions<TicketDb> options) : DbContext(options)
{
    public DbSet<Ticket> Tickets => Set<Ticket>();
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Ticket>().Property(x => x.Price).HasConversion<double>();
    }
}
public sealed class TicketPricingService(TicketDb db)
{
    public async Task<(int Updated, decimal ConfirmedPrice)> ApplyHolidayPriceAsync(int ticketId, decimal price)
    {
        var ticket = await db.Tickets.SingleAsync(x => x.Id == ticketId);
        var updated = await db.Tickets.Where(x => x.Id == ticketId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Price, price));
        return (updated, ticket.Price);
    }
}
