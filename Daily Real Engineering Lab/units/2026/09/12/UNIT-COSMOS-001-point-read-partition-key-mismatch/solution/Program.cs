record Ticket(string Id, string PartitionKey, string TenantCode, string Subject);

sealed class CosmosLikeContainer
{
    private readonly List<Ticket> _items =
    [
        new("case-1042", "tenant-acme", "ACME", "Checkout callback failed"),
        new("case-2048", "tenant-globe", "GLOBE", "Invoice export delayed")
    ];

    public Ticket? ReadItem(string id, string partitionKey) =>
        _items.SingleOrDefault(x => x.Id == id && x.PartitionKey == partitionKey);
}

static class TenantPartitionKey
{
    public static string FromTenantCode(string tenantCode) =>
        $"tenant-{tenantCode.Trim().ToLowerInvariant()}";
}

sealed class TicketRepository(CosmosLikeContainer container)
{
    public Ticket? Get(string id, string tenantCode) =>
        container.ReadItem(id, TenantPartitionKey.FromTenantCode(tenantCode));
}

var mode = args.FirstOrDefault() ?? "known";
var repository = new TicketRepository(new CosmosLikeContainer());
var id = mode == "unknown" ? "case-9999" : "case-1042";
var ticket = repository.Get(id, "ACME");
Console.WriteLine(ticket is null
    ? $"STATUS=404|ID={id}"
    : $"STATUS=200|ID={ticket.Id}|PK={ticket.PartitionKey}|TENANT={ticket.TenantCode}");