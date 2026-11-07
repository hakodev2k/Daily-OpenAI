using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddScoped<CatalogSession>();
services.AddSingleton<CatalogWorker>();
using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = false });

var worker = provider.GetRequiredService<CatalogWorker>();
worker.RunCycle(1, "v1");
worker.RunCycle(2, "v2");

public sealed class CatalogSession
{
    private string? _trackedVersion;
    public string Read(string databaseVersion) => _trackedVersion ??= databaseVersion;
}

public sealed class CatalogWorker
{
    private readonly CatalogSession _session;
    public CatalogWorker(CatalogSession session) => _session = session;

    public void RunCycle(int cycle, string databaseVersion)
    {
        var observed = _session.Read(databaseVersion);
        Console.WriteLine($"cycle={cycle}; database={databaseVersion}; observed={observed}; session={_session.GetHashCode()}");
    }
}