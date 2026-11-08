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
    // Lab policy: proxy address explicitly configured; do not trust headers from arbitrary peers.
    var peer=ctx.Connection.RemoteIpAddress;
    if(peer is null) return "unknown";
    if(!IPAddress.IsLoopback(peer)) return peer.ToString();
    var forwarded=ctx.Request.Headers["X-Forwarded-For"].ToString();
    if(!IPAddress.TryParse(forwarded, out var parsed)) return peer.ToString();
    return parsed.ToString();
}
