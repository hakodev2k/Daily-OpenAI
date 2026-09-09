using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<FeatureCatalog>();
services.AddSingleton<EndpointRouter>(sp =>
    new EndpointRouter(sp.GetRequiredService<FeatureCatalog>()));

using var provider = services.BuildServiceProvider();
var catalog = provider.GetRequiredService<FeatureCatalog>();
var router = provider.GetRequiredService<EndpointRouter>();

Console.WriteLine($"update-path instance:  {catalog.InstanceId}");
Console.WriteLine($"route-path instance:   {router.CatalogInstanceId}");
Console.WriteLine($"route before update:   {router.CurrentRoute}");

catalog.CurrentRoute = "v2";

Console.WriteLine($"catalog after update:  {catalog.CurrentRoute}");
Console.WriteLine($"route after update:    {router.CurrentRoute}");

var healthy = catalog.InstanceId == router.CatalogInstanceId
    && router.CurrentRoute == "v2";

return healthy ? 0 : 1;

public sealed class FeatureCatalog
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public string CurrentRoute { get; set; } = "v1";
}

public sealed class EndpointRouter
{
    private readonly FeatureCatalog _catalog;

    public EndpointRouter(FeatureCatalog catalog) => _catalog = catalog;

    public Guid CatalogInstanceId => _catalog.InstanceId;
    public string CurrentRoute => _catalog.CurrentRoute;
}
