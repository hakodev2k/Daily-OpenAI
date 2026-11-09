using System.IO.Compression;
using MuseumArchive;
using System.Text;

var root = Path.Combine(Path.GetTempPath(), "museum-lab-" + Guid.NewGuid().ToString("N"));
var intake = Path.Combine(root, "intake");
var outside = Path.Combine(root, "records.txt");
Directory.CreateDirectory(root);
try
{
    var zip = Path.Combine(root, "incoming.zip");
    using (var archive = ZipFile.Open(zip, ZipArchiveMode.Create))
    {
        using (var writer = new StreamWriter(archive.CreateEntry("../records.txt").Open(), Encoding.UTF8))
            writer.Write("CORRUPTED");
        using (var writer = new StreamWriter(archive.CreateEntry("images/catalog.txt").Open(), Encoding.UTF8))
            writer.Write("VALID");
    }
    try { ArchiveImporter.Import(zip, intake); }
    catch (Exception ex) { Console.WriteLine("IMPORT_EXCEPTION=" + ex.GetType().Name); }
    var escaped = File.Exists(outside);
    Console.WriteLine("ESCAPED=" + escaped);
    Console.WriteLine("VALID=" + File.Exists(Path.Combine(intake, "images", "catalog.txt")));
    if (args.Contains("--reproduce")) return escaped ? 0 : 1;
    if (escaped) return 2;
    var validZip = Path.Combine(root, "valid.zip");
    using (var archive = ZipFile.Open(validZip, ZipArchiveMode.Create))
    using (var writer = new StreamWriter(archive.CreateEntry("images/catalog.txt").Open()))
        writer.Write("VALID");
    var clean = Path.Combine(root, "clean");
    ArchiveImporter.Import(validZip, clean);
    return File.ReadAllText(Path.Combine(clean, "images", "catalog.txt")) == "VALID" ? 0 : 3;
}
finally { Directory.Delete(root, recursive: true); }
