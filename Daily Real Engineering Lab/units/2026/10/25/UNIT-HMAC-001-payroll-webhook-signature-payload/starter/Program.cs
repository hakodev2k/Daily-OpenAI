using System.Security.Cryptography;
using System.Text;

var mode = args.Length == 0 ? "run" : args[0].ToLowerInvariant();
if (mode is not ("run" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [run|reproduce|verify]");
    return 2;
}

var results = LabCases.Build()
    .Select(item => new Outcome(
        item.Name,
        item.ExpectedValid,
        WebhookVerifier.Verify(item.Body, item.SignatureHex, LabCases.Secret)))
    .ToArray();

foreach (var result in results)
{
    Console.WriteLine($"{result.Name,-21} expected={result.ExpectedValid,-5} actual={result.ActualValid,-5} match={result.ExpectedValid == result.ActualValid}");
}

if (mode == "run") return 0;

if (mode == "reproduce")
{
    var positiveMismatch = results.Any(x => x.ExpectedValid && !x.ActualValid);
    var positiveControl = results.Any(x => x.ExpectedValid && x.ActualValid);
    var negativeControls = results.Where(x => !x.ExpectedValid).All(x => !x.ActualValid);
    var reproduced = positiveMismatch && positiveControl && negativeControls;
    Console.WriteLine(reproduced ? "REPRODUCED: valid callback rejected." : "NOT REPRODUCED: inspect fixtures and starter.");
    return reproduced ? 0 : 1;
}

var passed = results.All(x => x.ExpectedValid == x.ActualValid);
Console.WriteLine(passed ? "VERIFY PASS: all signed and tampered cases satisfy the contract." : "VERIFY FAIL: one or more cases violate the contract.");
return passed ? 0 : 1;

internal sealed record Outcome(string Name, bool ExpectedValid, bool ActualValid);
internal sealed record WebhookCase(string Name, byte[] Body, string SignatureHex, bool ExpectedValid);

internal static class LabCases
{
    internal static readonly byte[] Secret = Encoding.UTF8.GetBytes("local-lab-secret-not-for-production");

    internal static IReadOnlyList<WebhookCase> Build()
    {
        var compact = Encoding.UTF8.GetBytes("""{"eventId":"evt-2001","status":"approved","amount":125.50}""");
        var pretty = Encoding.UTF8.GetBytes("{\n  \"eventId\": \"evt-2002\",\n  \"status\": \"approved\",\n  \"amount\": 125.50\n}");
        var spaced = Encoding.UTF8.GetBytes("""{ "eventId" : "evt-2003", "status" : "approved", "amount" : 125.50 }""");
        var tampered = Encoding.UTF8.GetBytes("""{"eventId":"evt-2001","status":"rejected","amount":125.50}""");

        static string Sign(byte[] payload) =>
            Convert.ToHexString(HMACSHA256.HashData(Secret, payload));

        return new[]
        {
            new WebhookCase("compact-valid", compact, Sign(compact), true),
            new WebhookCase("pretty-valid", pretty, Sign(pretty), true),
            new WebhookCase("spaced-valid", spaced, Sign(spaced), true),
            new WebhookCase("tampered-body", tampered, Sign(compact), false),
            new WebhookCase("malformed-signature", compact, "NOT-HEX", false),
            new WebhookCase("truncated-signature", compact, "ABCD", false)
        };
    }
}
