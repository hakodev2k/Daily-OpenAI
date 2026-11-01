using System.Diagnostics;
using System.Text.RegularExpressions;
using FerryCheckIn;

using var listener = new ActivityListener
{
    ShouldListenTo = source => source.Name == "Ferry.CheckIn",
    Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllDataAndRecorded,
    SampleUsingParentId = (ref ActivityCreationOptions<string> options) => ActivitySamplingResult.AllDataAndRecorded
};
ActivitySource.AddActivityListener(listener);

var request = new CheckInRequest("req-742", "mina.test@invalid.example", "7319", "G4");
var output = CheckInPipeline.Execute(request);
var baggage = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
foreach (var part in output.BaggageHeader.Split(',', StringSplitOptions.RemoveEmptyEntries))
{
    var separator = part.IndexOf('=');
    if (separator <= 0) continue;
    var key = Uri.UnescapeDataString(part[..separator]);
    var value = Uri.UnescapeDataString(part[(separator + 1)..]);
    baggage[key] = value;
}

var correlationOk = baggage.TryGetValue("request_id", out var correlation) &&
                    correlation == request.RequestId &&
                    Regex.IsMatch(output.TraceId, "^[0-9a-f]{32}$", RegexOptions.IgnoreCase);
var piiLeaked = baggage.ContainsKey("passenger_email") ||
                baggage.ContainsKey("passport_last4") ||
                output.BaggageHeader.Contains("mina.test", StringComparison.OrdinalIgnoreCase) ||
                output.BaggageHeader.Contains("7319", StringComparison.Ordinal);
var logSafe = !output.LogLine.Contains(request.PassengerEmail, StringComparison.OrdinalIgnoreCase) &&
              !output.LogLine.Contains(request.PassportLast4, StringComparison.Ordinal);
var businessOk = output.BusinessResult == "ACCEPTED";

Console.WriteLine($"TRACE_ID={output.TraceId}");
Console.WriteLine($"OUTBOUND_BAGGAGE={output.BaggageHeader}");
Console.WriteLine($"SERVICE_LOG={output.LogLine}");
Console.WriteLine($"BUSINESS_RESULT={output.BusinessResult}");
Console.WriteLine($"CORRELATION_OK={correlationOk}");
Console.WriteLine($"SENSITIVE_DATA_EXPORTED={piiLeaked}");
Console.WriteLine($"LOCAL_LOG_SAFE={logSafe}");

var mode = args.Length == 0 ? "run" : args[0].ToLowerInvariant();
if (mode == "reproduce")
{
    if (!piiLeaked || !correlationOk || !businessOk || !logSafe)
    {
        Console.Error.WriteLine("REPRODUCE_FAIL: starter symptom not observed.");
        return 2;
    }
    Console.WriteLine("REPRODUCE_PASS: privacy incident reproduced in local trace export.");
    return 0;
}
if (mode == "verify")
{
    if (piiLeaked || !correlationOk || !businessOk || !logSafe)
    {
        Console.Error.WriteLine("VERIFY_FAIL: trace privacy, correlation or business contract violated.");
        return 3;
    }
    Console.WriteLine("VERIFY_PASS: correlation retained, sensitive data absent, check-in accepted.");
    return 0;
}
if (mode != "run")
{
    Console.Error.WriteLine("Usage: run | reproduce | verify");
    return 4;
}
return 0;
