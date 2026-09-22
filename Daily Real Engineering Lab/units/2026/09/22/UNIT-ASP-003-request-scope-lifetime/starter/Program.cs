using Microsoft.Extensions.DependencyInjection;

var mode = args.SkipWhile(x => x != "--mode").Skip(1).FirstOrDefault() ?? "run";
var services = new ServiceCollection();
services.AddScoped<RequestContext>();
services.AddSingleton<OrderAuditService>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions
{
    ValidateScopes = false
});

var observed = new List<string>();
foreach (var tenant in new[] { "tenant-a", "tenant-b" })
{
    using var scope = provider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<RequestContext>();
    context.TenantId = tenant;

    var audit = scope.ServiceProvider.GetRequiredService<OrderAuditService>();
    var actual = audit.Record();
    observed.Add(actual);
    Console.WriteLine($"request={tenant}; observed={actual}; context={context.InstanceId}");
}

var mismatch = observed[0] != "tenant-a" || observed[1] != "tenant-b";
if (mode == "reproduce")
{
    Console.WriteLine(mismatch ? "EXPECTED_MISMATCH" : "PROBLEM_NOT_REPRODUCED");
    return mismatch ? 0 : 2;
}

if (mode == "verify")
{
    Console.WriteLine(mismatch ? "VERIFY_FAILED" : "VERIFY_OK");
    return mismatch ? 1 : 0;
}

return 0;

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