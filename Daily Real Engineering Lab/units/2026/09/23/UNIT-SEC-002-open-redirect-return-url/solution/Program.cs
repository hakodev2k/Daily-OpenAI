var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => Results.Text("Portal home"));
app.MapGet("/profile", () => Results.Text("Employee profile"));

app.MapPost("/login", (HttpRequest request) =>
{
    var returnUrl = request.Query["returnUrl"].FirstOrDefault();
    var destination = IsLocalUrl(returnUrl) ? returnUrl! : "/";
    return Results.Redirect(destination);
});

app.Run("http://127.0.0.1:5092");

static bool IsLocalUrl(string? url)
{
    if (string.IsNullOrEmpty(url)) return false;
    if (url[0] == '/') return url.Length == 1 || (url[1] != '/' && url[1] != '\\');
    return url.Length > 1 && url[0] == '~' && url[1] == '/';
}