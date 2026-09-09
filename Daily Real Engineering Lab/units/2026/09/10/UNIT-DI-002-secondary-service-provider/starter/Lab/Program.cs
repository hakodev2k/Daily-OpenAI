using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<FeatureCatalog>();

// Investigation note:
// Compare the object graph used here with the graph used after registration completes.
using var bootstrapProvider = services.BuildServiceProvider();
var bootstrapCatalog = bootstrapProvider.GetRequiredService<FeatureCatalog>();
services.AddSingleton(new EndpointRouter(bootstrapCatalog));

using var provider = services.BuildServiceProvider();
var catalog = provider.GetRequiredService<FeatureCatalog>();
var router = provider.GetRequiredService<EndpointRouter>();

Console.WriteLine($"update-path instance:  {catalog.InstanceId}");
Console.WriteLine($"route-path instance:   {router.CatalogInstanceId}");
Console.WriteLine($"route before update:   {router.CurrentRoute}");

catalog.CurrentRoute = "v2";

Console.WriteLine($"catalog after update:  {catalog.CurrentRoute}");
Console.WriteLine($"route after update:    {router.CurrentRoute}");

var expectedHealthyGraph = catalog.InstanceId == router.CatalogInstanceId
    && router.CurrentRoute == "v2";

return expectedHealthyGraph ? 0 : 1;

public sealed class FeatureCatalog
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public string CurrentRoute { get; set; } = "v1";
}

public sealed class EndpointRouter
{
    private readonly FeatureCatalog _catalog;

    public EndpointRouter(FeatureCatalog catalog)
    {
        _catalog = catalog;
    }

    public Guid CatalogInstanceId => _catalog.InstanceId;
    public string CurrentRoute => _catalog.CurrentRoute;
}
