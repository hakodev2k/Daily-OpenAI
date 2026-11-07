using System.Net;
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5098");
builder.Services.AddHttpClient("partner", client => client.BaseAddress = new Uri("http://127.0.0.1:5098"))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = true, CookieContainer = new CookieContainer() });
var app = builder.Build();
app.MapGet("/partner/session", (HttpRequest request, HttpResponse response) =>
{
    var value = request.Headers.Cookie.ToString();
    if (string.IsNullOrWhiteSpace(value)) response.Cookies.Append("LegacySession", "tenant-a");
    return Results.Text(string.IsNullOrWhiteSpace(value) ? "<none>" : value);
});
app.MapGet("/lab", async (IHttpClientFactory factory) =>
{
    using var first = factory.CreateClient("partner");
    await first.GetStringAsync("/partner/session");
    using var second = factory.CreateClient("partner");
    var observed = await second.GetStringAsync("/partner/session");
    return Results.Text("SECOND_CALL_COOKIE=" + observed);
});
app.Run();
