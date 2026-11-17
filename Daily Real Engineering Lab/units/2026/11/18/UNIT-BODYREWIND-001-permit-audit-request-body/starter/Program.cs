using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

var mode = args.FirstOrDefault() ?? "--inspect";
if (mode is not ("--inspect" or "--reproduce" or "--verify")) return 2;
const string payload = """{"permitId":"P-2048","parcel":"LOT-17"}""";
var bytes = Encoding.UTF8.GetBytes(payload);
var context = new DefaultHttpContext();
context.Request.Method = "POST";
context.Request.ContentType = "application/json";
context.Request.ContentLength = bytes.Length;
context.Request.Body = new MemoryStream(bytes, writable: false);
var auditPayload = "";
var handlerPayload = "";
var status = 0;
RequestDelegate handler = async ctx =>
{
    using var reader = new StreamReader(ctx.Request.Body, Encoding.UTF8, leaveOpen: true);
    handlerPayload = await reader.ReadToEndAsync();
    var valid = false;
    try
    {
        using var json = JsonDocument.Parse(handlerPayload);
        valid = json.RootElement.GetProperty("permitId").GetString() == "P-2048";
    }
    catch (JsonException) { }
    ctx.Response.StatusCode = valid ? 202 : 400;
    status = ctx.Response.StatusCode;
    Console.WriteLine($"HANDLER bytes={Encoding.UTF8.GetByteCount(handlerPayload)} status={status}");
};
RequestDelegate auditMiddleware = async ctx =>
{
    using var reader = new StreamReader(ctx.Request.Body, Encoding.UTF8, leaveOpen: true);
    auditPayload = await reader.ReadToEndAsync();
    Console.WriteLine($"AUDIT bytes={Encoding.UTF8.GetByteCount(auditPayload)}");
    await handler(ctx);
};
await auditMiddleware(context);
var auditOk = auditPayload == payload;
var handlerOk = handlerPayload == payload && status == 202;
if (mode == "--inspect")
{
    Console.WriteLine($"OBSERVED auditOk={auditOk} handlerOk={handlerOk}");
    return 0;
}
if (mode == "--reproduce")
{
    var reproduced = auditOk && !handlerOk && status == 400;
    Console.WriteLine(reproduced ? "REPRODUCE PASS" : "REPRODUCE FAIL");
    return reproduced ? 0 : 1;
}
var verified = auditOk && handlerOk;
Console.WriteLine(verified ? "VERIFY PASS" : "VERIFY FAIL");
return verified ? 0 : 1;
