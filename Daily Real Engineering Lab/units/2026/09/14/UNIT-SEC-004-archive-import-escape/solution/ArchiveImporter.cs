using System.IO.Compression;

public sealed class ArchiveImporter
{
    public void Extract(string archivePath, string destinationRoot)
    {
        var rootFullPath = Path.GetFullPath(destinationRoot);
        var rootWithSeparator = rootFullPath.EndsWith(Path.DirectorySeparatorChar)
            ? rootFullPath
            : rootFullPath + Path.DirectorySeparatorChar;

        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        Directory.CreateDirectory(rootFullPath);

        using var archive = ZipFile.OpenRead(archivePath);

        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name))
            {
                continue;
            }

            var targetPath = Path.GetFullPath(Path.Combine(rootFullPath, entry.FullName));

            if (!targetPath.StartsWith(rootWithSeparator, comparison))
            {
                continue;
            }

            var directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            entry.ExtractToFile(targetPath, overwrite: true);
        }
    }
}
