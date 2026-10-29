using System.IO.Compression;
namespace HatcheryIntake;
public static class ArchiveIntake
{
    public static void Extract(Stream archiveStream, string destinationDirectory)
    {
        ArgumentNullException.ThrowIfNull(archiveStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);
        var root = Path.GetFullPath(destinationDirectory);
        var prefix = Path.TrimEndingDirectorySeparator(root) + Path.DirectorySeparatorChar;
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        using var archive = new ZipArchive(archiveStream, ZipArchiveMode.Read, leaveOpen: true);
        var plan = new List<(ZipArchiveEntry Entry, string Target, bool Directory)>();
        foreach (var entry in archive.Entries)
        {
            var name = entry.FullName;
            if (string.IsNullOrWhiteSpace(name) || name.Contains('\\') || name.StartsWith('/')
                || (name.Length >= 2 && char.IsLetter(name[0]) && name[1] == ':'))
                throw new InvalidDataException("Unsupported ZIP entry name.");
            var relative = name.Replace('/', Path.DirectorySeparatorChar);
            var target = Path.GetFullPath(Path.Combine(root, relative));
            if (!target.StartsWith(prefix, comparison))
                throw new InvalidDataException("ZIP entry escapes intake root.");
            bool directory = name.EndsWith("/", StringComparison.Ordinal);
            if (directory && entry.Length != 0)
                throw new InvalidDataException("Directory entry contains data.");
            plan.Add((entry, target, directory));
        }
        Directory.CreateDirectory(root);
        foreach (var (entry, target, directory) in plan)
        {
            if (directory) Directory.CreateDirectory(target);
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                entry.ExtractToFile(target, overwrite: false);
            }
        }
    }
}
