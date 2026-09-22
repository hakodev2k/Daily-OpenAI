var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => Results.Text("Portal home"));
app.MapGet("/profile", () => Results.Text("Employee profile"));

app.MapPost("/login", (HttpRequest request) =>
{
    var returnUrl = request.Query["returnUrl"].FirstOrDefault();

    // Investigation note:
    // Which values are allowed to become a navigation destination after login?
    var destination = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
    return Results.Redirect(destination);
});

app.Run("http://127.0.0.1:5092");