using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddScoped<RequestContext>();
services.AddScoped<OrderAuditService>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

foreach (var tenant in new[] { "tenant-a", "tenant-b" })
{
    using var scope = provider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<RequestContext>();
    context.TenantId = tenant;
    var audit = scope.ServiceProvider.GetRequiredService<OrderAuditService>();
    Console.WriteLine($"request={tenant}; observed={audit.Record()}; context={context.InstanceId}");
}

public sealed class RequestContext
{
    public Guid InstanceId { get; } = Guid.NewGuid();
    public string TenantId { get; set; } = "unset";
}

public sealed class OrderAuditService
{
    private readonly RequestContext _context;
    public OrderAuditService(RequestContext context) => _context = context;
    public string Record() => _context.TenantId;
}