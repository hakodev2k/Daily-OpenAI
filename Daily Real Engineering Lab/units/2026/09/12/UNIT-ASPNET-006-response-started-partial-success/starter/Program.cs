using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new
        {
            title = "Export failed",
            status = 500
        });
    });
});

app.MapGet("/reports/orders.csv", async (HttpContext context, bool fail = false) =>
{
    context.Response.ContentType = "text/csv";
    context.Response.StatusCode = StatusCodes.Status200OK;

    await context.Response.WriteAsync("OrderId,Amount\n", Encoding.UTF8, context.RequestAborted);
    await context.Response.Body.FlushAsync(context.RequestAborted);

    for (var i = 1; i <= 5; i++)
    {
        if (fail && i == 3)
        {
            throw new InvalidOperationException("Simulated export failure after partial write.");
        }

        await context.Response.WriteAsync($"ORD-{i:000},{i * 10}\n", Encoding.UTF8, context.RequestAborted);
        await context.Response.Body.FlushAsync(context.RequestAborted);
    }
});

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run("http://127.0.0.1:5062");
