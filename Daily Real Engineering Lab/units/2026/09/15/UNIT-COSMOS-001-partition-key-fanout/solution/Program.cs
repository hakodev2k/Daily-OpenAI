using System.Collections.Concurrent;

var store = SeedStore(40, 8);
const string tenantId = "tenant-17";
const string invoiceId = "invoice-03";

var result = store.ReadItem(tenantId, invoiceId);

Console.WriteLine($"FOUND_INVOICE={result.Document?.Id ?? "<none>"}");
Console.WriteLine($"FOUND_TENANT={result.Document?.TenantId ?? "<none>"}");
Console.WriteLine($"PARTITIONS_TOUCHED={result.PartitionsTouched}");
Console.WriteLine($"SIMULATED_REQUEST_UNITS={result.RequestUnits}");

var correct = result.Document is { TenantId: tenantId, Id: invoiceId };
return correct && result.PartitionsTouched == 1 && result.RequestUnits == 1 ? 0 : 1;

static PartitionedDocumentStore SeedStore(int tenantCount, int invoicesPerTenant)
{
    var store = new PartitionedDocumentStore();
    for (var t = 0; t < tenantCount; t++)
    {
        var tenant = $"tenant-{t:00}";
        for (var i = 0; i < invoicesPerTenant; i++)
        {
            store.Add(new InvoiceDocument(tenant, $"invoice-{i:00}", 100 + i));
        }
    }

    return store;
}

internal sealed record InvoiceDocument(string TenantId, string Id, decimal Total);
internal sealed record ReadResult(InvoiceDocument? Document, int PartitionsTouched, int RequestUnits);

internal sealed class PartitionedDocumentStore
{
    private readonly ConcurrentDictionary<string, List<InvoiceDocument>> _partitions = new();

    public void Add(InvoiceDocument document)
    {
        var partition = _partitions.GetOrAdd(document.TenantId, _ => []);
        partition.Add(document);
    }

    public ReadResult ReadItem(string tenantId, string invoiceId)
    {
        if (!_partitions.TryGetValue(tenantId, out var partition))
        {
            return new ReadResult(null, 1, 1);
        }

        var found = partition.FirstOrDefault(x => x.Id == invoiceId);
        return new ReadResult(found, 1, 1);
    }
}
