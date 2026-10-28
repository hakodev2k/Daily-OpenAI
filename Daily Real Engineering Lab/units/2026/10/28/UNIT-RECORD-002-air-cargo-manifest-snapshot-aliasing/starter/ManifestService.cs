namespace ManifestAudit;

public sealed record ManifestSnapshot(
    string FlightId,
    IReadOnlyList<string> BagTags,
    DateTimeOffset CapturedAtUtc);

public sealed class ManifestService
{
    private readonly Dictionary<string, ManifestSnapshot> _flights = new(StringComparer.Ordinal);

    public void OpenFlight(string flightId, DateTimeOffset at)
    {
        if (!_flights.TryAdd(flightId, new ManifestSnapshot(flightId, new List<string>(), at)))
            throw new InvalidOperationException($"Flight already exists: {flightId}");
    }

    public void ScanBag(string flightId, string bagTag, DateTimeOffset at)
    {
        var current = _flights[flightId];
        var next = current with { CapturedAtUtc = at };
        ((List<string>)next.BagTags).Add(bagTag);
        _flights[flightId] = next;
    }

    public ManifestSnapshot Capture(string flightId, DateTimeOffset at)
    {
        return _flights[flightId] with { CapturedAtUtc = at };
    }
}
