using System.Text.RegularExpressions;

const int AdversarialLength = 28;
const string Pattern = "^(a+)+$";
var validator = new Regex(
    Pattern,
    RegexOptions.CultureInvariant | RegexOptions.NonBacktracking,
    TimeSpan.FromMilliseconds(100));

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
    try
    {
        var actual = validator.IsMatch(testCase.Input);
        if (actual != testCase.Expected)
        {
            Console.WriteLine($"VERIFY_FAIL: correctness mismatch for length {testCase.Input.Length}");
            return 1;
        }
    }
    catch (RegexMatchTimeoutException)
    {
        Console.WriteLine($"VERIFY_FAIL: timeout for length {testCase.Input.Length}");
        return 2;
    }
}

Console.WriteLine("VERIFY_PASS");
return 0;
