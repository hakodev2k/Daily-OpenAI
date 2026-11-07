using System.IO.Compression;
using System.Text;

namespace HeritageArchive;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
                throw new ArgumentException("Argument phải là reproduce hoặc verify.");
            if (args[0] == "reproduce") Reproduce();
            else Verify();
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FAILED: {ex.GetType().Name}: {ex.Message}");
            return 1;
        }
    }

    private static void Reproduce()
    {
        using var f = new Fixture();
        f.WriteZip(("photos/manifest.txt", "safe"), ("../escaped.txt", "outside"));
        new ArchiveImportService().Extract(f.ZipPath, f.Destination);
        Assert(File.Exists(f.EscapePath), "Không thấy file ngoài extract.");
        Assert(File.Exists(f.ManifestPath), "File hợp lệ không được nhập.");
        Console.WriteLine($"REPRODUCED: output ngoài vùng nhập: {f.EscapePath}");
    }

    private static void Verify()
    {
        var importer = new ArchiveImportService();
        using (var f = new Fixture())
        {
            f.WriteZip(("photos/manifest.txt", "safe"), ("../escaped.txt", "outside"));
            MustReject(importer, f, "traversal");
            Assert(!File.Exists(f.EscapePath), "File đã được ghi ngoài extract.");
            Assert(!File.Exists(f.ManifestPath), "Batch không được ghi một phần trước khi bị từ chối.");
        }
        using (var f = new Fixture())
        {
            f.WriteZip(("../extract-sibling/escaped.txt", "outside"));
            MustReject(importer, f, "sibling-prefix");
            Assert(!File.Exists(Path.Combine(f.Root, "extract-sibling", "escaped.txt")),
                "File bị ghi ra sibling directory.");
        }
        using (var f = new Fixture())
        {
            f.WriteZip(("photos/manifest.txt", "safe"), ("photos/2026/index.txt", "ok"));
            importer.Extract(f.ZipPath, f.Destination);
            Assert(File.ReadAllText(f.ManifestPath) == "safe", "Manifest không đúng.");
            Assert(File.ReadAllText(Path.Combine(f.Destination, "photos", "2026", "index.txt")) == "ok",
                "Nested file hợp lệ không được nhập.");
        }
        Console.WriteLine("VERIFIED: từ chối ZIP nguy hiểm trước khi ghi và nhập thành công ZIP hợp lệ.");
    }

    private static void MustReject(ArchiveImportService importer, Fixture f, string caseName)
    {
        var rejected = false;
        try { importer.Extract(f.ZipPath, f.Destination); }
        catch (InvalidDataException) { rejected = true; }
        Assert(rejected, $"Phải từ chối {caseName} bằng InvalidDataException.");
    }

    private static void Assert(bool ok, string message)
    {
        if (!ok) throw new InvalidOperationException(message);
    }

    private sealed class Fixture : IDisposable
    {
        public string Root { get; } = Path.Combine(Path.GetTempPath(), "HeritageArchiveLab-" + Guid.NewGuid().ToString("N"));
        public string ZipPath => Path.Combine(Root, "batch.zip");
        public string Destination => Path.Combine(Root, "extract");
        public string EscapePath => Path.Combine(Root, "escaped.txt");
        public string ManifestPath => Path.Combine(Destination, "photos", "manifest.txt");

        public void WriteZip(params (string Name, string Text)[] entries)
        {
            Directory.CreateDirectory(Root);
            using var file = File.Create(ZipPath);
            using var zip = new ZipArchive(file, ZipArchiveMode.Create);
            foreach (var (name, text) in entries)
            {
                var entry = zip.CreateEntry(name);
                using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
                writer.Write(text);
            }
        }

        public void Dispose()
        {
            if (Directory.Exists(Root)) Directory.Delete(Root, recursive: true);
        }
    }
}
