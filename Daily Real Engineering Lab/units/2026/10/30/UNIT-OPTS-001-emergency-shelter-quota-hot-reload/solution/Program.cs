using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["Quota:MaxBedsPerRequest"] = "4"
    })
    .Build();

var services = new ServiceCollection();
services.AddOptions();
services.Configure<QuotaOptions>(configuration.GetSection("Quota"));
services.AddSingleton<BedReservationService>();

using var provider = services.BuildServiceProvider();
var reservationService = provider.GetRequiredService<BedReservationService>();

var results = new List<bool>();
Check("initial-policy", 3, true);
Check("reject-nonpositive", 0, false);

configuration["Quota:MaxBedsPerRequest"] = "2";
configuration.Reload();
Check("reduced-quota", 3, false);
Check("reduced-quota-allowed", 2, true);

configuration["Quota:MaxBedsPerRequest"] = "5";
configuration.Reload();
Check("increased-quota", 5, true);
Check("increased-quota-exceeded", 6, false);

if (results.All(static ok => ok))
{
    Console.WriteLine("VERIFIED: all six live-policy contract checks passed");
    return 0;
}

Console.WriteLine("CONFIG_DRIFT_DETECTED: at least one policy decision differs from active configuration");
return 23;

void Check(string name, int requestedBeds, bool expected)
{
    var actual = reservationService.CanReserve(requestedBeds);
    var current = configuration["Quota:MaxBedsPerRequest"];
    Console.WriteLine(
        $"check={name} configuredMax={current} requested={requestedBeds} expected={expected} actual={actual} result={(actual == expected ? "PASS" : "FAIL")}");
    results.Add(actual == expected);
}

public sealed class QuotaOptions
{
    public int MaxBedsPerRequest { get; set; }
}

public sealed class BedReservationService
{
    private readonly IOptionsMonitor<QuotaOptions> _options;

    public BedReservationService(IOptionsMonitor<QuotaOptions> options)
    {
        _options = options;
    }

    public bool CanReserve(int requestedBeds)
    {
        var maxBeds = _options.CurrentValue.MaxBedsPerRequest;
        if (maxBeds <= 0)
            throw new InvalidOperationException("Invalid active shelter quota configuration.");
        return requestedBeds > 0 && requestedBeds <= maxBeds;
    }
}
