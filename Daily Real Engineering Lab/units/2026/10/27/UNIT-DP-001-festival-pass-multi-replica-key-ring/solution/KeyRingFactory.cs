using Microsoft.AspNetCore.DataProtection;

namespace FestivalPass;

public static class KeyRingFactory
{
    public static IDataProtectionProvider Create(string replicaId, string keyRingDirectory)
    {
        if (string.IsNullOrWhiteSpace(keyRingDirectory))
            throw new ArgumentException("A shared key-ring directory is required.", nameof(keyRingDirectory));

        Directory.CreateDirectory(keyRingDirectory);
        return DataProtectionProvider.Create(
            new DirectoryInfo(keyRingDirectory),
            builder => builder.SetApplicationName("FestivalPassPortal"));
    }
}
