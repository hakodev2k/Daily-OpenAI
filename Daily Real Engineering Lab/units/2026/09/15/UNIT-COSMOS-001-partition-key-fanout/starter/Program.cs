using System.Collections.Concurrent;

var store = SeedStore(40, 8);
const string tenantId = "tenant-17";
const string invoiceId = "invoice-03";

// Investigation note:
// This API receives both tenantId and invoiceId. Observe how much of the store
// must be visited to return one document.
var result = store.QueryByInvoiceId(invoiceId);

Console.WriteLine($"FOUND_INVOICE={result.Document?.Id ?? "<none>"}");
Console.WriteLine($"FOUND_TENANT={result.Document?.TenantId ?? "<none>"}");
Console.WriteLine($"PARTITIONS_TOUCHED={result.PartitionsTouched}");
Console.WriteLine($"SIMULATED_REQUEST_UNITS={result.RequestUnits}");

var correct = result.Document is { TenantId: tenantId, Id: invoiceId };
var mode = args.FirstOrDefault() ?? "--run";

if (mode.Equals("--reproduce", StringComparison.OrdinalIgnoreCase))
{
    var symptomExists = correct && result.PartitionsTouched > 1;
    Console.WriteLine(symptomExists ? "REPRODUCED=YES" : "REPRODUCED=NO");
    return symptomExists ? 0 : 1;
}

if (mode.Equals("--verify", StringComparison.OrdinalIgnoreCase))
{
    var fixedBehavior = correct && result.PartitionsTouched == 1 && result.RequestUnits == 1;
    Console.WriteLine(fixedBehavior ? "VERIFICATION=PASS" : "VERIFICATION=FAIL");
    return fixedBehavior ? 0 : 1;
}

return 0;

static PartitionedDocumentStore SeedStore(int tenantCount, int invoicesPerTenant)
{
    var store = new PartitionedDocumentStore();
    for (var t = 0; t < tenantCount; t++)
    {
        var tenantId = $"tenant-{t:00}";
        for (var i = 0; i < invoicesPerTenant; i++)
        {
            store.Add(new InvoiceDocument(tenantId, $"invoice-{i:00}", 100 + i));
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

    public ReadResult QueryByInvoiceId(string invoiceId)
    {
        var touched = 0;
        InvoiceDocument? found = null;

        foreach (var partition in _partitions.OrderBy(x => x.Key))
        {
            touched++;
            found = partition.Value.FirstOrDefault(x => x.Id == invoiceId && x.TenantId == "tenant-17");
            if (found is not null)
            {
                break;
            }
        }

        return new ReadResult(found, touched, touched);
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
