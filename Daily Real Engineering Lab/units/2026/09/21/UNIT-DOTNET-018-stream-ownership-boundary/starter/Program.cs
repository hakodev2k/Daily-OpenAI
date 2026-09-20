using System.Text;

static void AppendFooter(Stream output)
{
    using (output)
    using (var writer = new StreamWriter(output, Encoding.UTF8, leaveOpen: true))
    {
        writer.WriteLine("TOTAL,2");
        writer.Flush();
    }
}

static string UploadSimulation(Stream payload)
{
    payload.Position = 0;
    using var reader = new StreamReader(payload, Encoding.UTF8, leaveOpen: true);
    return reader.ReadToEnd();
}

var stream = new MemoryStream();
using (var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
{
    writer.WriteLine("ID,AMOUNT");
    writer.WriteLine("A-100,40");
    writer.WriteLine("A-101,60");
    writer.Flush();
}

AppendFooter(stream);

try
{
    var uploaded = UploadSimulation(stream);
    Console.WriteLine(uploaded);
    if (args.Contains("--verify"))
        return uploaded.Contains("A-100,40") && uploaded.Contains("TOTAL,2") ? 0 : 3;
    return 0;
}
catch (ObjectDisposedException ex)
{
    Console.WriteLine($"{ex.GetType().Name}: downstream cannot read export payload");
    return args.Contains("--reproduce") ? 0 : 2;
}
finally
{
    stream.Dispose();
}