using System.Globalization;
using System.Runtime.CompilerServices;

namespace FreightManifest;

public sealed record ManifestLine(string ManifestId, string ContainerId, int PackageCount);

public sealed class SourceParser
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public ManifestLine Parse(string raw)
    {
        var fields = raw.Split('|');
        if (fields.Length != 3)
            throw new InvalidDataException("Manifest row must contain exactly three fields.");

        var packageCount = int.Parse(fields[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
        if (packageCount <= 0)
            throw new InvalidDataException("Package count must be positive.");

        return new ManifestLine(fields[0], fields[1], packageCount);
    }
}
