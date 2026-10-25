public enum PartnerResult { Accepted, Unauthorized, InvalidRequest }

public sealed record PartnerAttempt(string ReportId, string CredentialVersion, PartnerResult Result);

public sealed class PartnerEmulator
{
    private string _acceptedVersion = "v1";
    private readonly List<PartnerAttempt> _attempts = new();

    public IReadOnlyList<PartnerAttempt> Attempts => _attempts;

    public void RotateTo(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version must be non-empty.", nameof(version));
        _acceptedVersion = version;
    }

    public PartnerResult Accept(string reportId, string suppliedVersion)
    {
        var result = string.IsNullOrWhiteSpace(reportId)
            ? PartnerResult.InvalidRequest
            : string.Equals(suppliedVersion, _acceptedVersion, StringComparison.Ordinal)
                ? PartnerResult.Accepted
                : PartnerResult.Unauthorized;

        _attempts.Add(new PartnerAttempt(reportId, suppliedVersion, result));
        return result;
    }
}
