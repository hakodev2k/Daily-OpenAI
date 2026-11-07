using Microsoft.AspNetCore.DataProtection;

namespace FestivalPass;

public static class KeyRingFactory
{
    // Replica được cấu hình tại deployment; thư mục keyRingDirectory do operator cung cấp.
    public static IDataProtectionProvider Create(string replicaId, string keyRingDirectory)
    {
        // Implementation đang chạy trong môi trường hiện tại.
        return new EphemeralDataProtectionProvider();
    }
}
