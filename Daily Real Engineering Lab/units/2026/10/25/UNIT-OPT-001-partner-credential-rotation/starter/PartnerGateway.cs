using Microsoft.Extensions.Options;

public sealed class PartnerGateway
{
    private readonly PartnerEmulator _partner;
    private readonly string _credentialVersion;

    public PartnerGateway(IOptions<PartnerOptions> options, PartnerEmulator partner)
    {
        _partner = partner;
        _credentialVersion = options.Value.CredentialVersion;
    }

    public PartnerResult Send(string reportId)
    {
        return _partner.Accept(reportId, _credentialVersion);
    }
}
