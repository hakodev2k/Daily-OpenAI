using System.Diagnostics;
using System.Text.RegularExpressions;

const int AdversarialLength = 28;
const string Pattern = "^(a+)+$";
var validator = new Regex(
    Pattern,
    RegexOptions.CultureInvariant,
    TimeSpan.FromMilliseconds(100));

var mode = args.FirstOrDefault() ?? "--run";

return mode switch
{
    "--reproduce" => Reproduce(validator),
    "--verify" => Verify(validator),
    _ => RunExamples(validator)
};

static int RunExamples(Regex validator)
{
    foreach (var input in new[] { "a", "aaaaaa", "abc", new string('a', 20) + "!" })
    {
        var result = Evaluate(validator, input);
        Console.WriteLine($"length={input.Length}, result={result}");
    }

    return 0;
}

static int Reproduce(Regex validator)
{
    var normal = Measure(validator, "aaaaaa");
    var simpleInvalid = Measure(validator, "abc");
    var adversarial = Measure(validator, new string('a', AdversarialLength) + "!");

    Console.WriteLine($"NORMAL={normal.Result};ELAPSED_MS={normal.ElapsedMilliseconds}");
    Console.WriteLine($"SIMPLE_INVALID={simpleInvalid.Result};ELAPSED_MS={simpleInvalid.ElapsedMilliseconds}");
    Console.WriteLine($"ADVERSARIAL={adversarial.Result};ELAPSED_MS={adversarial.ElapsedMilliseconds}");

    if (adversarial.Result == ValidationResult.Timeout)
    {
        Console.WriteLine("REGEX_TIMEOUT=1");
        return 0;
    }

    Console.WriteLine("REGEX_TIMEOUT=0");
    Console.WriteLine("If your machine does not reproduce the timeout, increase AdversarialLength gradually.");
    return 1;
}

static int Verify(Regex validator)
{
    var cases = new (string Input, bool Expected)[]
    {
        ("a", true),
        ("aaaaaa", true),
        ("", false),
        ("abc", false),
        ("aaaa!", false),
        (new string('a', AdversarialLength) + "!", false)
    };

    foreach (var testCase in cases)
    {
        var measured = Measure(validator, testCase.Input);
        if (measured.Result == ValidationResult.Timeout)
        {
            Console.WriteLine($"VERIFY_FAIL: timeout for input length {testCase.Input.Length}");
            return 2;
        }

        var actual = measured.Result == ValidationResult.Match;
        if (actual != testCase.Expected)
        {
            Console.WriteLine($"VERIFY_FAIL: correctness mismatch for input length {testCase.Input.Length}");
            return 3;
        }
    }

    Console.WriteLine("VERIFY_PASS");
    return 0;
}

static (ValidationResult Result, long ElapsedMilliseconds) Measure(Regex validator, string input)
{
    var stopwatch = Stopwatch.StartNew();
    var result = Evaluate(validator, input);
    stopwatch.Stop();
    return (result, stopwatch.ElapsedMilliseconds);
}

static ValidationResult Evaluate(Regex validator, string input)
{
    try
    {
        return validator.IsMatch(input) ? ValidationResult.Match : ValidationResult.NoMatch;
    }
    catch (RegexMatchTimeoutException)
    {
        return ValidationResult.Timeout;
    }
}

enum ValidationResult
{
    Match,
    NoMatch,
    Timeout
}
