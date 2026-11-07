namespace AccessCodeLab;

public sealed class AccessCodeRegistry
{
    // Contract: ASCII case-insensitive machine identifier, independent of UI locale.
    private readonly Dictionary<string, string> _seatByCode =
        new(StringComparer.OrdinalIgnoreCase);

    public bool TryAdd(string code, string seat) =>
        _seatByCode.TryAdd(code, seat);

    public bool TryGetSeat(string code, out string? seat) =>
        _seatByCode.TryGetValue(code, out seat);
}
