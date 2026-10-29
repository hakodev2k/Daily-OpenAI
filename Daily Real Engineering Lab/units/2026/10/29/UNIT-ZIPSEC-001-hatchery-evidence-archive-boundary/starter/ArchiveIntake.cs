using System.IO.Compression;
namespace HatcheryIntake;
public static class ArchiveIntake
{
    public static void Extract(Stream archiveStream, string destinationDirectory)
    {
        Directory.CreateDirectory(destinationDirectory);
        using var archive = new ZipArchive(archiveStream, ZipArchiveMode.Read, leaveOpen: true);
        foreach (var entry in archive.Entries)
        {
            var targetPath = Path.Combine(destinationDirectory, entry.FullName);
            if (string.IsNullOrEmpty(entry.Name))
            {
                Directory.CreateDirectory(targetPath);
                continue;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
            entry.ExtractToFile(targetPath, overwrite: false);
        }
    }
}
