using System.Text;

namespace EncodingLab;

public static class PayloadPolicy
{
    public static bool CanSend(string message, int maxPayloadBytes)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentOutOfRangeException.ThrowIfNegative(maxPayloadBytes);

        return Encoding.UTF8.GetByteCount(message) <= maxPayloadBytes;
    }
}
