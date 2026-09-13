using System.Globalization;

var mode = args.FirstOrDefault() ?? "reproduce";
var originalCulture = CultureInfo.CurrentCulture;

try
{
    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");

    var configuredIds = new[] { "INVOICE" };
    var incomingId = "invoice";

    // Investigation note: đây là identifier kỹ thuật, không phải văn bản hiển thị cho người dùng.
    bool IsKnown(string candidate) => configuredIds.Any(x => x.ToUpper() == candidate.ToUpper());

    var matched = IsKnown(incomingId);
    Console.WriteLine($"Culture={CultureInfo.CurrentCulture.Name}");
    Console.WriteLine($"Configured={configuredIds[0]}, Incoming={incomingId}");
    Console.WriteLine($"Matched={matched}");

    if (mode.Equals("verify", StringComparison.OrdinalIgnoreCase))
    {
        return matched ? 0 : 1;
    }

    return matched ? 0 : 2;
}
finally
{
    CultureInfo.CurrentCulture = originalCulture;
}
