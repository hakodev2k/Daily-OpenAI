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
    // Reference strategy for this bounded report: complete the fallible work
    // before committing a success response.
    var csv = new StringBuilder();
    csv.AppendLine("OrderId,Amount");

    for (var i = 1; i <= 5; i++)
    {
        if (fail && i == 3)
        {
            throw new InvalidOperationException("Simulated export failure before response commit.");
        }

        csv.AppendLine($"ORD-{i:000},{i * 10}");
    }

    context.Response.ContentType = "text/csv";
    context.Response.StatusCode = StatusCodes.Status200OK;
    await context.Response.WriteAsync(csv.ToString(), Encoding.UTF8, context.RequestAborted);
});

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run("http://127.0.0.1:5063");
