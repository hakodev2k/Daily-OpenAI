using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var mode = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();
if (mode is not ("demo" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [demo|reproduce|verify]");
    return 64;
}

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["Partner:CredentialVersion"] = "v1"
    })
    .Build();

var services = new ServiceCollection();
services.AddOptions<PartnerOptions>().Bind(configuration.GetSection("Partner"));
services.AddSingleton<PartnerEmulator>();
services.AddSingleton<PartnerGateway>();

using var provider = services.BuildServiceProvider(validateScopes: true);
var partner = provider.GetRequiredService<PartnerEmulator>();
var gateway = provider.GetRequiredService<PartnerGateway>();

var first = gateway.Send("report-1001");

partner.RotateTo("v2");
configuration["Partner:CredentialVersion"] = "v2";
configuration.Reload();
var configV2 = configuration["Partner:CredentialVersion"];
var controlV2 = partner.Accept("control-v2", "v2");
var second = gateway.Send("report-1002");

partner.RotateTo("v3");
configuration["Partner:CredentialVersion"] = "v3";
configuration.Reload();
var configV3 = configuration["Partner:CredentialVersion"];
var controlV3 = partner.Accept("control-v3", "v3");
var third = gateway.Send("report-1003");
var oldCredentialControl = partner.Accept("control-stale-v1", "v1");

var sentV2 = partner.Attempts.Single(a => a.ReportId == "report-1002");
var sentV3 = partner.Attempts.Single(a => a.ReportId == "report-1003");

Console.WriteLine($"initial={first} config=v1");
Console.WriteLine($"rotation2 provider={configV2} partnerControl={controlV2} gateway={second} sentVersion={sentV2.CredentialVersion}");
Console.WriteLine($"rotation3 provider={configV3} partnerControl={controlV3} gateway={third} sentVersion={sentV3.CredentialVersion}");
Console.WriteLine($"oldVersionControl={oldCredentialControl}");

bool controlsOk = first == PartnerResult.Accepted
    && configV2 == "v2" && configV3 == "v3"
    && controlV2 == PartnerResult.Accepted
    && controlV3 == PartnerResult.Accepted
    && oldCredentialControl == PartnerResult.Unauthorized;

if (!controlsOk)
{
    Console.Error.WriteLine("CONTROL_FAILED: simulation invariant is broken.");
    return 20;
}

if (mode == "reproduce")
{
    bool incident = second == PartnerResult.Unauthorized
        && third == PartnerResult.Unauthorized
        && sentV2.CredentialVersion == "v1"
        && sentV3.CredentialVersion == "v1";
    Console.WriteLine(incident ? "INCIDENT_REPRODUCED" : "INCIDENT_NOT_REPRODUCED");
    return incident ? 0 : 21;
}

if (mode == "verify")
{
    bool fixedBehavior = second == PartnerResult.Accepted
        && third == PartnerResult.Accepted
        && sentV2.CredentialVersion == "v2"
        && sentV3.CredentialVersion == "v3";
    Console.WriteLine(fixedBehavior ? "VERIFICATION_PASSED" : "VERIFICATION_FAILED");
    return fixedBehavior ? 0 : 22;
}

return 0;
