using System.Diagnostics;
using System.Text.RegularExpressions;

const int MinLength = 18;
const int MaxLength = 34;

var validator = new SlugValidator();

Console.WriteLine($"VALID_RESULT={validator.IsValid("product-alpha")}");
Console.WriteLine($"INVALID_RESULT={validator.IsValid("Product Alpha!")}");

var timeoutCount = 0;
for (var length = MinLength; length <= MaxLength; length += 2)
{
    var candidate = new string('a', length) + "!";
    var stopwatch = Stopwatch.StartNew();

    try
    {
        var result = validator.IsValid(candidate);
        stopwatch.Stop();
        Console.WriteLine($"LEN={length};RESULT={result};ELAPSED_MS={stopwatch.Elapsed.TotalMilliseconds:F1}");
    }
    catch (RegexMatchTimeoutException)
    {
        stopwatch.Stop();
        timeoutCount++;
        Console.WriteLine($"LEN={length};RESULT=TIMEOUT;ELAPSED_MS={stopwatch.Elapsed.TotalMilliseconds:F1}");
    }
}

Console.WriteLine($"ADVERSARIAL_TIMEOUTS={timeoutCount}");

internal sealed class SlugValidator
{
    private static readonly Regex Pattern = new(
        "^[a-z]+$",
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking,
        TimeSpan.FromMilliseconds(75));

    public bool IsValid(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Pattern.IsMatch(value);
    }
}
