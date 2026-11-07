using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ParkingPermit.Reference;

public static class PermitRoutes
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/api/permits/{id:int}", (int id) =>
            Results.Ok(new { permitId = id, status = "ACTIVE" }));

        // Unmatched API paths must not be mistaken for successful SPA navigation.
        app.Map("/api/{**path}", () =>
            Results.NotFound(new { code = "api_route_not_found" }));

        app.MapFallback(() => Results.Content(
            "<!doctype html><html><body><main id='app'>Parking permit portal</main></body></html>",
            "text/html; charset=utf-8"));
    }
}
