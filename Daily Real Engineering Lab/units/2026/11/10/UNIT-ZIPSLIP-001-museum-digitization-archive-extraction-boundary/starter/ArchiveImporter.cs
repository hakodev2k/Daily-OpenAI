using System.IO.Compression;

namespace MuseumArchive;

public static class ArchiveImporter
{
    public static void Import(string zipPath, string destination)
    {
        Directory.CreateDirectory(destination);
        using var archive = ZipFile.OpenRead(zipPath);
        foreach (var entry in archive.Entries)
        {
            var output = Path.Combine(destination, entry.FullName);
            if (string.IsNullOrEmpty(entry.Name))
            {
                Directory.CreateDirectory(output);
                continue;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(output)!);
            entry.ExtractToFile(output, overwrite: true);
        }
    }
}
