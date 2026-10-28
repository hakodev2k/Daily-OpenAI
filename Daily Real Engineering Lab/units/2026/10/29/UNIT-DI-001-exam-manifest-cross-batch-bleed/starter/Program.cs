using Microsoft.Extensions.DependencyInjection;

var mode = args.Length > 0 ? args[0].ToLowerInvariant() : "run";
if (mode is not ("run" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [run|reproduce|verify]");
    return 2;
}

var services = new ServiceCollection();
services.AddScoped<BatchComposer>();
services.AddSingleton<ManifestExportWorker>();

try
{
    using var provider = services.BuildServiceProvider(new ServiceProviderOptions
    {
        ValidateScopes = mode == "verify",
        ValidateOnBuild = mode == "verify"
    });
    var worker = provider.GetRequiredService<ManifestExportWorker>();
    var morning = worker.Export(new ExportBatch("AM-EXAM", new[] { "A01", "A02" }));
    var afternoon = worker.Export(new ExportBatch("PM-EXAM", new[] { "B03" }));
    Console.WriteLine("Morning:   " + string.Join(", ", morning));
    Console.WriteLine("Afternoon: " + string.Join(", ", afternoon));

    var morningCorrect = morning.SequenceEqual(new[] { "AM-EXAM|A01", "AM-EXAM|A02" });
    var afternoonCorrect = afternoon.SequenceEqual(new[] { "PM-EXAM|B03" });
    if (mode == "reproduce")
    {
        var reproduced = morningCorrect && !afternoonCorrect;
        Console.WriteLine(reproduced ? "REPRODUCED: independent-batch contract violated." : "NOT REPRODUCED.");
        return reproduced ? 0 : 1;
    }
    if (mode == "verify")
    {
        var correct = morningCorrect && afternoonCorrect;
        Console.WriteLine(correct ? "PASS: independent exports and strict DI validation." : "FAIL: wrong export contents.");
        return correct ? 0 : 1;
    }
    return 0;
}
catch (Exception ex) when (mode == "verify" && (ex is AggregateException || ex is InvalidOperationException))
{
    Console.Error.WriteLine("FAIL: strict DI service graph validation.");
    Console.Error.WriteLine(ex.Message);
    return 1;
}

public sealed record ExportBatch(string SessionId, string[] SeatIds);

public sealed class BatchComposer
{
    private readonly List<string> _rows = new();

    public string[] Compose(ExportBatch batch)
    {
        foreach (var seat in batch.SeatIds)
        {
            _rows.Add(string.Concat(batch.SessionId, "|", seat));
        }
        return _rows.ToArray();
    }
}

public sealed class ManifestExportWorker
{
    private readonly BatchComposer _composer;

    public ManifestExportWorker(BatchComposer composer)
    {
        _composer = composer;
    }

    public string[] Export(ExportBatch batch) => _composer.Compose(batch);
}
