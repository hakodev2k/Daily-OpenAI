using System.Net;
using Microsoft.AspNetCore.Http;
var builder=WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5193");
var app=builder.Build();
app.MapGet("/admin/permits",(HttpContext ctx)=>{
    var client=ResolveClient(ctx);
    return client=="203.0.113.7" ? Results.Ok(new {status="approved",client}) : Results.StatusCode(403);
});
app.MapGet("/diagnostics/peer",(HttpContext ctx)=>Results.Ok(new {peer=ctx.Connection.RemoteIpAddress?.ToString(),claimed=ctx.Request.Headers["X-Forwarded-For"].ToString()}));
app.Run();
string ResolveClient(HttpContext ctx) {
    var forwarded=ctx.Request.Headers["X-Forwarded-For"].ToString();
    return !string.IsNullOrWhiteSpace(forwarded) ? forwarded.Split(',')[0].Trim() : ctx.Connection.RemoteIpAddress?.ToString()??"unknown";
}
