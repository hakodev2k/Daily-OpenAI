var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(o => o.AddPolicy("PartnerPortal", p =>
    p.WithOrigins("https://portal.insurer.test")
     .WithMethods("POST")
     .WithHeaders("Content-Type", "Authorization")));
var app = builder.Build();
app.UseCors("PartnerPortal");
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.MapPost("/api/claims/preview", () => Results.Ok(new { previewId = "pv-104", status = "ready" }));
// Fake endpoint only: no real authentication or sensitive data.
app.Run();
