using System.Globalization;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddEnvironmentVariables(prefix: "BAGGAGE_")
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

var alertMinutes = configuration.GetValue<int>("Baggage:AlertAfterMinutes");
var escalationQueue = configuration["Baggage:EscalationQueue"] ?? "(missing)";

var expectedMinutesText = Environment.GetEnvironmentVariable("BAGGAGE_Baggage__AlertAfterMinutes");
var expectedQueueText = Environment.GetEnvironmentVariable("BAGGAGE_Baggage__EscalationQueue");
var expectedMinutes = expectedMinutesText is null
    ? 30
    : int.Parse(expectedMinutesText, CultureInfo.InvariantCulture);
var expectedQueue = expectedQueueText ?? "standard";

Console.WriteLine("EXPECTED_ALERT_MINUTES=" + expectedMinutes);
Console.WriteLine("EFFECTIVE_ALERT_MINUTES=" + alertMinutes);
Console.WriteLine("EXPECTED_ESCALATION_QUEUE=" + expectedQueue);
Console.WriteLine("EFFECTIVE_ESCALATION_QUEUE=" + escalationQueue);

if (args.Contains("--assert-contract", StringComparer.Ordinal))
{
    if (alertMinutes != expectedMinutes || escalationQueue != expectedQueue)
    {
        Console.Error.WriteLine("CONTRACT_FAIL: runtime settings disagree with deployment contract.");
        return 2;
    }

    Console.WriteLine("CONTRACT_PASS");
}

return 0;
