using System.Globalization;
using Microsoft.AspNetCore.OutputCaching;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("closure-notices", policy => policy
        .Expire(TimeSpan.FromMinutes(10))
        .SetVaryByHeader("Accept-Language"));
});

var app = builder.Build();
app.UseOutputCache();

var renderCount = 0;
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/api/pool/closure", (HttpContext context) =>
{
    var requestedLanguage = context.Request.Headers["Accept-Language"].ToString();
    var isVietnamese = requestedLanguage.StartsWith("vi", StringComparison.OrdinalIgnoreCase);
    var count = Interlocked.Increment(ref renderCount);

    context.Response.Headers["X-Render-Count"] = count.ToString(CultureInfo.InvariantCulture);
    return Results.Json(new PoolNotice(
        "POOL-2026-1030",
        isVietnamese ? "vi" : "en",
        isVietnamese
            ? "Hồ bơi đóng cửa từ 14:00 để kiểm tra chất lượng nước."
            : "The pool closes at 14:00 for water-quality inspection.",
        count));
}).CacheOutput("closure-notices");

app.Run();

record PoolNotice(string NoticeId, string Language, string Message, int RenderCount);
