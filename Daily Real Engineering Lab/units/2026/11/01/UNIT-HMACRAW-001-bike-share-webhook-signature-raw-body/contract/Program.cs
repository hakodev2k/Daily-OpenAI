using System.Security.Cryptography;
using System.Text;

if (args.Length != 1 || !new[] { "demo", "reproduce", "verify" }.Contains(args[0]))
{
    Console.Error.WriteLine("Usage: dotnet run -- [demo|reproduce|verify]");
    return 64;
}
byte[] key = Encoding.UTF8.GetBytes("lab-only-key-never-use-in-production");
var verifier = new WebhookVerifier(key);
byte[] compact = Encoding.UTF8.GetBytes("{\"dockId\":\"D-17\",\"bikeId\":\"B-004\",\"state\":\"returned\",\"battery\":54}");
byte[] pretty = Encoding.UTF8.GetBytes("{\n  \"dockId\": \"D-17\",\n  \"bikeId\": \"B-004\",\n  \"state\": \"returned\",\n  \"battery\": 54\n}");
byte[] reordered = Encoding.UTF8.GetBytes("{\"battery\":54,\"state\":\"returned\",\"bikeId\":\"B-004\",\"dockId\":\"D-17\"}");
byte[] tampered = Encoding.UTF8.GetBytes("{\"dockId\":\"D-17\",\"bikeId\":\"B-004\",\"state\":\"missing\",\"battery\":54}");
string Sign(byte[] bytes) => Convert.ToHexString(HMACSHA256.HashData(key, bytes));
var cases = new[]
{
    new ContractCase("compact-valid", compact, Sign(compact), true),
    new ContractCase("pretty-valid", pretty, Sign(pretty), true),
    new ContractCase("reordered-valid", reordered, Sign(reordered), true),
    new ContractCase("tampered-invalid", tampered, Sign(compact), false),
    new ContractCase("malformed-signature", compact, "NOT-HEX", false),
    new ContractCase("wrong-signature", compact, new string('0', 64), false)
};
var observations = new Dictionary<string, bool>(StringComparer.Ordinal);
bool allMatch = true;
foreach (var test in cases)
{
    bool accepted = verifier.Verify(test.Body, test.Signature);
    observations.Add(test.Name, accepted);
    bool matches = accepted == test.Expected;
    allMatch &= matches;
    Console.WriteLine($"{test.Name}: accepted={accepted}, expected={test.Expected}, {(matches ? "OK" : "MISMATCH")}");
}
if (args[0] == "reproduce")
{
    bool symptom = observations["compact-valid"] && !observations["pretty-valid"] && !observations["tampered-invalid"];
    Console.WriteLine(symptom ? "REPRODUCED: valid signed payload rejected" : "NOT REPRODUCED");
    return symptom ? 0 : 2;
}
if (args[0] == "verify")
{
    Console.WriteLine(allMatch ? "PASS: signature contract" : "FAIL: signature contract");
    return allMatch ? 0 : 1;
}
return 0;

internal sealed record ContractCase(string Name, byte[] Body, string Signature, bool Expected);
