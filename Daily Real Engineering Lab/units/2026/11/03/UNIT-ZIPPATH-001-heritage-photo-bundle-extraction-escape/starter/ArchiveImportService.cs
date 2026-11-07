using System.IO.Compression;

namespace HeritageArchive;

public sealed class ArchiveImportService
{
    public void Extract(string zipPath, string destinationDirectory)
    {
        using var archive = ZipFile.OpenRead(zipPath);
        foreach (var entry in archive.Entries)
        {
            if (entry.Name.Length == 0)
                continue;

            var outputPath = Path.Combine(destinationDirectory, entry.FullName);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            entry.ExtractToFile(outputPath, overwrite: true);
        }
    }
}
