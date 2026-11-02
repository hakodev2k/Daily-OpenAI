using System.IO.Compression;

namespace HeritageArchive;

public sealed class ArchiveImportService
{
    public void Extract(string zipPath, string destinationDirectory)
    {
        using var archive = ZipFile.OpenRead(zipPath);
        var root = Path.GetFullPath(destinationDirectory);
        var rootWithSeparator = Path.TrimEndingDirectorySeparator(root) + Path.DirectorySeparatorChar;
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var comparer = OperatingSystem.IsWindows()
            ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

        var seen = new HashSet<string>(comparer);
        var planned = new List<(ZipArchiveEntry Entry, string Path)>();
        foreach (var entry in archive.Entries)
        {
            var name = entry.FullName.Replace('\\', '/');
            if (name.StartsWith('/') ||
                (name.Length >= 2 && char.IsLetter(name[0]) && name[1] == ':'))
                throw new InvalidDataException("Archive entry contains an absolute path.");

            var outputPath = Path.GetFullPath(
                Path.Combine(root, name.Replace('/', Path.DirectorySeparatorChar)));
            if (!outputPath.StartsWith(rootWithSeparator, comparison))
                throw new InvalidDataException("Archive entry escapes extraction directory.");

            if (entry.Name.Length == 0) continue;
            if (!seen.Add(outputPath))
                throw new InvalidDataException("Archive contains duplicate destination paths.");
            planned.Add((entry, outputPath));
        }

        Directory.CreateDirectory(root);
        foreach (var (entry, path) in planned)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            entry.ExtractToFile(path, overwrite: false);
        }
    }
}
