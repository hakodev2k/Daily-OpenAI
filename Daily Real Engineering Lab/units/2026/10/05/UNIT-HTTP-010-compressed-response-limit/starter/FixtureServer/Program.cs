using System.IO.Compression;
using System.Net;
using System.Text;

var listener = new HttpListener();
listener.Prefixes.Add("http://127.0.0.1:5088/");
listener.Start();
Console.WriteLine("Fixture server listening on http://127.0.0.1:5088/");

while (true)
{
    var context = await listener.GetContextAsync();
    if (context.Request.Url?.AbsolutePath != "/export")
    {
        context.Response.StatusCode = 404;
        context.Response.Close();
        continue;
    }

    var payload = Encoding.UTF8.GetBytes("REPORT-2026|" + new string('A', 8_000_000));
    context.Response.StatusCode = 200;
    context.Response.ContentType = "application/octet-stream";
    context.Response.AddHeader("Content-Encoding", "gzip");

    await using (var gzip = new GZipStream(context.Response.OutputStream, CompressionLevel.SmallestSize, leaveOpen: true))
    {
        await gzip.WriteAsync(payload);
    }

    context.Response.Close();
}
