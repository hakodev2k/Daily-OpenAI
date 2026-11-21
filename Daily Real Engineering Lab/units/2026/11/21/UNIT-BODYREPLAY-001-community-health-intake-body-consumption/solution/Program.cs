using System.Text;
using Microsoft.AspNetCore.Http;
var cases = new[] { "{\"id\":\"A1\",\"note\":\"Follow-up\"}", "{\"id\":\"B2\",\"note\":\"Tái khám\"}", "{\"id\":\"C3\",\"consent\":true}" };
var mode = args.FirstOrDefault() ?? "reproduce";
int wrong = 0;
foreach (var payload in cases) {
    var ctx = new DefaultHttpContext();
    ctx.Request.Method = "POST";
    ctx.Request.ContentType = "application/json";
    ctx.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(payload));
    string received = "";
    await Audit.Invoke(ctx, async request => {
        using var reader = new StreamReader(request.Request.Body, Encoding.UTF8, leaveOpen: true);
        received = await reader.ReadToEndAsync();
    });
    Console.WriteLine($"expected={payload.Length} received={received.Length} equal={payload == received}");
    if (payload != received) wrong++;
}
if (mode == "reproduce") { if (wrong == 0) Environment.ExitCode = 1; else Console.WriteLine($"REPRODUCED {wrong} mismatches"); }
else if (mode == "verify") { if (wrong != 0) Environment.ExitCode = 1; else Console.WriteLine("VERIFY PASS"); }
else Environment.ExitCode = 2;
static class Audit {
    public static async Task Invoke(HttpContext ctx, RequestDelegate next) {
        ctx.Request.EnableBuffering();
        using (var reader = new StreamReader(ctx.Request.Body, Encoding.UTF8, leaveOpen: true)) {
            var audit = await reader.ReadToEndAsync();
            Console.WriteLine($"auditChars={audit.Length}");
        }
        ctx.Request.Body.Position = 0;
        await next(ctx);
    }
}
