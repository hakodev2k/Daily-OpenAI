using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<BatchLedger>();
services.AddScoped<InvoiceRepository>();
services.AddSingleton<IrrigationInvoiceWorker>();
using var provider = services.BuildServiceProvider(validateScopes: false);
var worker = provider.GetRequiredService<IrrigationInvoiceWorker>();
var ledger = provider.GetRequiredService<BatchLedger>();
var mode = args.FirstOrDefault() ?? "--reproduce";
try
{
    worker.Process("garden-north");
    worker.Process("garden-south");
    if (mode == "--reproduce") throw new Exception("Expected the second batch to fail.");
    if (ledger.Completed.SequenceEqual(new[] { "garden-north", "garden-south" }))
    {
        Console.WriteLine("VERIFIED: both batches committed.");
        return;
    }
    throw new Exception("Batch ledger is incomplete.");
}
catch (ObjectDisposedException) when (mode == "--reproduce" && ledger.Completed.SequenceEqual(new[] { "garden-north" }))
{
    Console.WriteLine("REPRODUCED: first batch committed; second batch failed after resource disposal.");
    return;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"FAILED: {ex.GetType().Name}: {ex.Message}");
    Environment.ExitCode = 1;
}

sealed class BatchLedger
{
    public List<string> Completed { get; } = new();
}

sealed class InvoiceRepository : IDisposable
{
    private bool _disposed;
    private readonly BatchLedger _ledger;
    public InvoiceRepository(BatchLedger ledger) => _ledger = ledger;
    public void Commit(string batch)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _ledger.Completed.Add(batch);
    }
    public void Dispose() => _disposed = true;
}

sealed class IrrigationInvoiceWorker
{
    private readonly InvoiceRepository _repository;
    public IrrigationInvoiceWorker(IServiceScopeFactory scopeFactory)
    {
        using var scope = scopeFactory.CreateScope();
        _repository = scope.ServiceProvider.GetRequiredService<InvoiceRepository>();
    }
    public void Process(string batch) => _repository.Commit(batch);
}
