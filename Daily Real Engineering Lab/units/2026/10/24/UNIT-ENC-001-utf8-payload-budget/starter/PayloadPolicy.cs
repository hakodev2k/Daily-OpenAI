namespace EncodingLab;

/// <summary>
/// Validates a message against the provider's configured payload budget.
/// The provider itself is simulated in Program.cs and must not be modified.
/// </summary>
public static class PayloadPolicy
{
    public static bool CanSend(string message, int maxPayloadBytes)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentOutOfRangeException.ThrowIfNegative(maxPayloadBytes);

        // Investigation: is this measurement defined in the same units
        // as the downstream provider's payload contract?
        return message.Length <= maxPayloadBytes;
    }
}
