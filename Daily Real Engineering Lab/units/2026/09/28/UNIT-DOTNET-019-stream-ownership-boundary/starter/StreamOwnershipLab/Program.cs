using StreamOwnershipLab;
var service = new ExportService();
try
{
    await using var export = await service.CreateInvoiceExportAsync(42);
    Console.WriteLine($"Caller: CanRead={export.CanRead}");
    using var reader = new StreamReader(export);
    var payload = await reader.ReadToEndAsync();
    Console.WriteLine($"Caller: Payload={payload}");
}
catch (Exception ex)
{
    Console.WriteLine($"Caller: {ex.GetType().Name}: {ex.Message}");
}