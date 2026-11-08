using System.Text;

var web = "Caf\u00e9-72";
var mobile = "Cafe\u0301-72";
var distinct = "Cafe-72";

string IdentityKey(string submitted) => submitted.Trim();

bool TryRegister(HashSet<string> existing, string submitted)
    => existing.Add(IdentityKey(submitted));

void Show(string channel, string submitted)
{
    Console.WriteLine($"{channel}: chars={submitted.Length} bytes={Encoding.UTF8.GetByteCount(submitted)} codepoints={string.Join(",", submitted.EnumerateRunes().Select(r => r.Value.ToString("X4")))}");
}
Show("web", web);
Show("mobile", mobile);

var registered = new HashSet<string>(StringComparer.Ordinal);
var first = TryRegister(registered, web);
var second = TryRegister(registered, mobile);
var third = TryRegister(registered, distinct);
Console.WriteLine($"first={first}, duplicateAccepted={second}, distinctAccepted={third}, count={registered.Count}");
var mode = args.Length == 1 ? args[0] : "verify";
if (mode == "reproduce")
{
    if (first && second && third && registered.Count == 3)
    {
        Console.WriteLine("REPRODUCED: duplicate-looking identifiers both accepted.");
        return 0;
    }
    Console.Error.WriteLine("REPRODUCTION NOT OBSERVED.");
    return 1;
}
if (mode != "verify") return 2;
if (!first || second || !third || registered.Count != 2)
{
    Console.Error.WriteLine("VERIFY FAILED: identity uniqueness or distinct-key regression.");
    return 1;
}
Console.WriteLine("VERIFY PASSED: equivalent identifiers collide; distinct remains allowed.");
return 0;
