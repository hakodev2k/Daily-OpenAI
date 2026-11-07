using Microsoft.Extensions.Options;

public sealed class PartnerGateway
{
    private readonly PartnerEmulator _partner;
    private readonly IOptionsMonitor<PartnerOptions> _options;

    public PartnerGateway(IOptionsMonitor<PartnerOptions> options, PartnerEmulator partner)
    {
        _options = options;
        _partner = partner;
    }

    public PartnerResult Send(string reportId)
    {
        // Chụp một phiên bản cho đúng một operation; không cache qua nhiều operation.
        string version = _options.CurrentValue.CredentialVersion;
        if (string.IsNullOrWhiteSpace(version))
            return PartnerResult.Unauthorized;

        return _partner.Accept(reportId, version);
    }
}
