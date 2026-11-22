using System.Diagnostics;
using System.Text.RegularExpressions;

var mode = args.Length > 0 ? args[0] : "normal";
var code = mode switch
{
    "normal" => new string('a', 18),
    "adversarial" => new string('a', 18) + "!",
    "verify" => new string('a', 18) + "!",
    _ => mode
};
var stopwatch = Stopwatch.StartNew();
try
{
    var accepted = ReferenceCodeValidator.IsValid(code);
    stopwatch.Stop();
    Console.WriteLine($"accepted={accepted};elapsedMs={stopwatch.ElapsedMilliseconds};length={code.Length}");
    if (mode == "verify" && (accepted || stopwatch.ElapsedMilliseconds > 2000)) Environment.ExitCode = 2;
}
catch (RegexMatchTimeoutException)
{
    stopwatch.Stop();
    Console.WriteLine($"accepted=false;timeout=true;elapsedMs={stopwatch.ElapsedMilliseconds}");
    if (mode == "verify") Environment.ExitCode = 2;
}

public static class ReferenceCodeValidator
{
    public static bool IsValid(string code)
    {
        return Regex.IsMatch(code, @"^(a+)+$");
    }
}
