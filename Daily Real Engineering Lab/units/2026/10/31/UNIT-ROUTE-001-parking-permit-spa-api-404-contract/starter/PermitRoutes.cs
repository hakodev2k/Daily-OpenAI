using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ParkingPermit.Starter;

public static class PermitRoutes
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/api/permits/{id:int}", (int id) =>
            Results.Ok(new { permitId = id, status = "ACTIVE" }));

        // Cổng phục vụ cả API và giao diện single-page application.
        app.MapFallback(() => Results.Content(
            "<!doctype html><html><body><main id='app'>Parking permit portal</main></body></html>",
            "text/html; charset=utf-8"));
    }
}
