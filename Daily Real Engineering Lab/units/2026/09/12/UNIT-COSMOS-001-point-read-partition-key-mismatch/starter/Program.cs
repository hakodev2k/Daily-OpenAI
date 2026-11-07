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

    public IEnumerable<Ticket> QueryById(string id) => _items.Where(x => x.Id == id);
}

sealed class TicketRepository(CosmosLikeContainer container)
{
    public Ticket? Get(string id, string tenantCode)
    {
        // Investigation note: compare the address used by this point read
        // with the address carried by the stored item.
        var partitionKey = tenantCode;
        return container.ReadItem(id, partitionKey);
    }
}

var mode = args.FirstOrDefault() ?? "known";
var container = new CosmosLikeContainer();
var repository = new TicketRepository(container);

if (mode == "inspect")
{
    foreach (var item in container.QueryById("case-1042"))
        Console.WriteLine($"SEED ID={item.Id}|PK={item.PartitionKey}|TENANT={item.TenantCode}");
    return;
}

var id = mode == "unknown" ? "case-9999" : "case-1042";
var ticket = repository.Get(id, "ACME");

if (ticket is null)
{
    Console.WriteLine($"STATUS=404|ID={id}");
    return;
}

Console.WriteLine($"STATUS=200|ID={ticket.Id}|PK={ticket.PartitionKey}|TENANT={ticket.TenantCode}");