using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/profile", () => Results.Text("{\"id\":42,\"name\":\"Lan\"}", "text/plain"));
app.MapGet("/consume", async () =>
{
    using var client = new HttpClient { BaseAddress = new Uri("http://127.0.0.1:5091") };
    var profile = await client.GetFromJsonAsync<Profile>("/profile");
    return Results.Json(profile);
});

app.Run("http://127.0.0.1:5091");
record Profile(int Id, string Name);