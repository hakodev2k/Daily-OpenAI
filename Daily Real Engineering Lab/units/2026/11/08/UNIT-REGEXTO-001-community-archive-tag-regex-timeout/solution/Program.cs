using System.Text.RegularExpressions;
var tags = new[] { "archive", "heritage", new string('a', 4000) + "!" };
foreach (var tag in tags)
{
    try
    {
        bool valid = Regex.IsMatch(tag, @"\A[a-z]{3,32}\z", RegexOptions.CultureInvariant | RegexOptions.NonBacktracking, TimeSpan.FromMilliseconds(40));
        Console.WriteLine($"tag.length={tag.Length} valid={valid}");
        if (valid != (tag is "archive" or "heritage")) { Environment.ExitCode = 1; return; }
    }
    catch (RegexMatchTimeoutException)
    {
        Console.Error.WriteLine($"TIMEOUT tag.length={tag.Length}");
        Environment.ExitCode = 2;
        return;
    }
}
