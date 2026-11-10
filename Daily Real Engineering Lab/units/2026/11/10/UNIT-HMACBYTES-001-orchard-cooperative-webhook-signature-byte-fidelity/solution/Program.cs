using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

internal static class Program
{
    private static readonly byte[] Secret = Encoding.UTF8.GetBytes("local-fixture-secret-not-for-production");
    private static readonly byte[] Compact = Encoding.UTF8.GetBytes("{\"ticket\":42,\"kg\":125}");
    private static readonly byte[] Pretty = Encoding.UTF8.GetBytes("{ \"kg\": 125, \"ticket\": 42 }");

    private static string Sign(ReadOnlySpan<byte> body) =>
        Convert.ToHexString(HMACSHA256.HashData(Secret, body));

    // Production adapter receives the request body and the X-Partner-Signature header.
    private static bool Authenticate(byte[] body, string header)
    {
        try
        {
            byte[] candidate = HMACSHA256.HashData(Secret, body);
            byte[] supplied = Convert.FromHexString(header);
            return supplied.Length == candidate.Length &&
                   CryptographicOperations.FixedTimeEquals(supplied, candidate);
        }
        catch (Exception ex) when (ex is JsonException or FormatException)
        {
            return false;
        }
    }

    private static int Main(string[] args)
    {
        var cases = new (string Name, byte[] Body, string Signature, bool Expected)[]
        {
            ("compact-valid", Compact, Sign(Compact), true),
            ("pretty-valid", Pretty, Sign(Pretty), true),
            ("modified-body", Encoding.UTF8.GetBytes("{\"ticket\":42,\"kg\":126}"), Sign(Compact), false),
            ("invalid-signature", Compact, new string('0', 64), false),
            ("malformed-signature", Compact, "NOT-HEX", false)
        };
        int failures = 0;
        foreach (var test in cases)
        {
            bool actual = Authenticate(test.Body, test.Signature);
            Console.WriteLine($"{test.Name}: bytes={test.Body.Length} actual={(actual ? 200 : 401)} expected={(test.Expected ? 200 : 401)}");
            if (actual != test.Expected) failures++;
        }

        bool reproduce = args.Length == 2 && args[0] == "--mode" && args[1] == "reproduce";
        if (reproduce)
        {
            Console.WriteLine(failures > 0 ? "REPRODUCED: valid webhook rejected" : "NOT REPRODUCED");
            return failures > 0 ? 0 : 1;
        }
        Console.WriteLine(failures == 0 ? "VERIFIED" : $"FAILED: {failures} contract(s)");
        return failures == 0 ? 0 : 1;
    }
}
