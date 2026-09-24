var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    context.Response.StatusCode = 500;
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync("{\"error\":\"report-generation-failed\"}");
}));

app.MapGet("/reports/{account}", async (string account, HttpContext context) =>
{
    context.Response.ContentType = "text/csv";
    await context.Response.WriteAsync("id,total\n");
    await context.Response.Body.FlushAsync();

    await Task.Delay(20);
    if (account == "problem")
        throw new InvalidOperationException("Simulated report source failure after initial output.");

    await context.Response.WriteAsync("A-100,42\nB-200,84\n");
});

app.Run();