using System.IO.Compression;
using System.Text;

if (args.Length != 2 || !new[] { "--fixture", "--extract" }.Contains(args[0]))
{
    Console.Error.WriteLine("Usage: --fixture|--extract <sandbox-root>");
    return 2;
}
var root = Path.GetFullPath(args[1]);
Directory.CreateDirectory(root);
var archive = Path.Combine(root, "intake.zip");
var destination = Path.GetFullPath(Path.Combine(root, "extracted"));
if (args[0] == "--fixture")
{
    using var stream = File.Create(archive);
    using var zip = new ZipArchive(stream, ZipArchiveMode.Create);
    Write(zip, "records/2026/report.txt", "Approved community record");
    Write(zip, "../outside.txt", "UNTRUSTED OVERRIDE");
    Console.WriteLine("Fixture ready: " + archive);
    return 0;
}
Directory.CreateDirectory(destination);
using (var zip = ZipFile.OpenRead(archive))
{
    foreach (var entry in zip.Entries)
    {
        if (string.IsNullOrEmpty(entry.Name)) continue;
        var target = Path.GetFullPath(Path.Combine(destination, entry.FullName));
        if (!target.StartsWith(destination + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine("Rejected entry: " + entry.FullName);
            continue;
        }
        Directory.CreateDirectory(Path.GetDirectoryName(target)!);
        using var input = entry.Open();
        using var output = File.Create(target);
        input.CopyTo(output);
        Console.WriteLine("Extracted: " + entry.FullName);
    }
}
return 0;
static void Write(ZipArchive zip, string name, string value)
{
    using var writer = new StreamWriter(zip.CreateEntry(name).Open(), new UTF8Encoding(false));
    writer.Write(value);
}
