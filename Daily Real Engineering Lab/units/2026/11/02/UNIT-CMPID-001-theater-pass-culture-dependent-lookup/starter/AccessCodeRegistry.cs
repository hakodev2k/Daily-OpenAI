namespace AccessCodeLab;

public sealed class AccessCodeRegistry
{
    // Ticket codes are used as keys for both admission and duplicate detection.
    private readonly Dictionary<string, string> _seatByCode =
        new(StringComparer.CurrentCultureIgnoreCase);

    public bool TryAdd(string code, string seat) =>
        _seatByCode.TryAdd(code, seat);

    public bool TryGetSeat(string code, out string? seat) =>
        _seatByCode.TryGetValue(code, out seat);
}
