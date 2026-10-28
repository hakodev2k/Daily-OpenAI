using Microsoft.EntityFrameworkCore;

namespace StreetlightLab;

public sealed class WorkOrder
{
    public int Id { get; set; }
    public string TenantId { get; set; } = "";
    public string Code { get; set; } = "";
    public bool IsOpen { get; set; }
}

public sealed class WorkOrdersContext : DbContext
{
    public WorkOrdersContext(DbContextOptions<WorkOrdersContext> options) : base(options) { }

    public string? TenantId { get; set; }
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkOrder>().HasKey(x => x.Id);
        modelBuilder.Entity<WorkOrder>()
            .HasQueryFilter(x => x.TenantId == TenantId);
    }
}
