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
    await Task.Delay(20);
    if(account == "problem") throw new InvalidOperationException("Simulated report preparation failure.");
    var csv = "id,total\nA-100,42\nB-200,84\n";
    context.Response.ContentType = "text/csv";
    await context.Response.WriteAsync(csv);
});
app.Run();