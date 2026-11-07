using Microsoft.AspNetCore.OutputCaching;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("tenant-flags", policy =>
        policy.Expire(TimeSpan.FromMinutes(5)));
});

var app = builder.Build();

app.UseOutputCache();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/flags", GetFlags).CacheOutput("tenant-flags");

app.Run();

static IResult GetFlags(HttpRequest request)
{
    var tenantId = request.Headers["X-Tenant-Id"].ToString();

    if (string.IsNullOrWhiteSpace(tenantId))
    {
        return Results.BadRequest(new { error = "X-Tenant-Id is required" });
    }

    var featurePlan = tenantId switch
    {
        "tenant-a" => "Premium",
        "tenant-b" => "Standard",
        _ => "Basic"
    };

    Console.WriteLine($"Endpoint executed for {tenantId} at {DateTimeOffset.UtcNow:O}");

    return Results.Ok(new
    {
        tenant = tenantId,
        featurePlan,
        generatedAt = DateTimeOffset.UtcNow
    });
}
