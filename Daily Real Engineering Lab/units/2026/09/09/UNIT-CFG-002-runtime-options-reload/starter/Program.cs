using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var initial = new Dictionary<string, string?>
{
    ["Routing:Endpoint"] = "https://primary.internal"
};

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(initial)
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
services.Configure<RoutingOptions>(configuration.GetSection("Routing"));
services.AddSingleton<RoutingClient>();

using var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<RoutingClient>();

Console.WriteLine($"REQUEST_BEFORE={client.GetEndpoint()}");

configuration["Routing:Endpoint"] = "https://failover.internal";
configuration.Reload();

Console.WriteLine($"CONFIG_AFTER={configuration["Routing:Endpoint"]}");
Console.WriteLine($"REQUEST_AFTER={client.GetEndpoint()}");

public sealed class RoutingOptions
{
    public string Endpoint { get; set; } = string.Empty;
}

public sealed class RoutingClient
{
    private readonly RoutingOptions _options;

    public RoutingClient(IOptions<RoutingOptions> options)
    {
        // Investigation note: what lifetime does this value effectively have?
        _options = options.Value;
    }

    public string GetEndpoint() => _options.Endpoint;
}
