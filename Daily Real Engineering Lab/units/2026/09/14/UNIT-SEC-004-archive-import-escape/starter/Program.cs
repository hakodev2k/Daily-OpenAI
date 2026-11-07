using System.IO.Compression;

var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".lab-output"));
var importRoot = Path.Combine(root, "import");
var packagePath = Path.Combine(root, "theme-package.zip");
var escapedPath = Path.Combine(root, "outside.txt");

if (Directory.Exists(root))
{
    Directory.Delete(root, recursive: true);
}

Directory.CreateDirectory(root);
Directory.CreateDirectory(importRoot);

using (var archive = ZipFile.Open(packagePath, ZipArchiveMode.Create))
{
    var valid = archive.CreateEntry("theme/index.html");
    await using (var writer = new StreamWriter(valid.Open()))
    {
        await writer.WriteAsync("<h1>Theme OK</h1>");
    }

    var unexpected = archive.CreateEntry("../outside.txt");
    await using (var writer = new StreamWriter(unexpected.Open()))
    {
        await writer.WriteAsync("should-not-be-written-outside-import-root");
    }
}

var importer = new ArchiveImporter();
importer.Extract(packagePath, importRoot);

var validPath = Path.Combine(importRoot, "theme", "index.html");
var validContentOk = File.Exists(validPath)
    && await File.ReadAllTextAsync(validPath) == "<h1>Theme OK</h1>";
var escaped = File.Exists(escapedPath);

Console.WriteLine($"Import root: {importRoot}");
Console.WriteLine($"Valid content present: {validContentOk}");
Console.WriteLine($"File outside import root present: {escaped}");

if (!validContentOk)
{
    Console.WriteLine("SAFE_CONTENT_MISSING");
    return 3;
}

if (escaped)
{
    Console.WriteLine("BOUNDARY_VIOLATION_DETECTED");
    return 2;
}

Console.WriteLine("IMPORT_CONTAINED");
return 0;
