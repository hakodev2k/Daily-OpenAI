using System.Globalization;
using PaperMillGauge;

var mode = args.FirstOrDefault() ?? "--report";
var report = RunContract();
foreach (var message in report.Messages)
    Console.WriteLine(message);
Console.WriteLine($"Passed={report.Passed}; Failed={report.Failed}; ControlPassed={report.ControlPassed}");

return mode switch
{
    "--report" => 0,
    "--expect-bug" => report.ControlPassed && report.Failed >= 2 ? 0 : 1,
    "--expect-fixed" => report.Failed == 0 ? 0 : 1,
    _ => 2
};

static ContractReport RunContract()
{
    var messages = new List<string>();
    var passed = 0;
    var failed = 0;

    bool Check(string label, Action action)
    {
        try
        {
            action();
            passed++;
            messages.Add($"PASS {label}");
            return true;
        }
        catch (Exception ex)
        {
            failed++;
            messages.Add($"FAIL {label}: {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }

    void AssertValid(string locale)
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(locale);
            var actual = GaugeImport.ParseLines(new[]
            {
                "ROLL-A;1.234",
                "ROLL-B;0.875",
                "ROLL-C;2.500"
            });
            var expected = new[] { 1.234m, 0.875m, 2.500m };
            if (actual.Count != expected.Length)
                throw new InvalidOperationException("Unexpected record count.");

            for (var i = 0; i < expected.Length; i++)
            {
                if (actual[i].RollId != $"ROLL-{(char)('A' + i)}" ||
                    actual[i].ThicknessMm != expected[i])
                    throw new InvalidOperationException(
                        $"Row {i + 1}: expected {expected[i].ToString(CultureInfo.InvariantCulture)} mm, " +
                        $"actual {actual[i].ThicknessMm.ToString(CultureInfo.InvariantCulture)} mm.");
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    void AssertRejected(string locale, string row)
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(locale);
            try
            {
                _ = GaugeImport.ParseLines(new[] { row });
            }
            catch (FormatException)
            {
                return;
            }
            throw new InvalidOperationException($"Invalid wire value was accepted: {row}");
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    var controlPassed = Check("en-US valid baseline", () => AssertValid("en-US"));
    Check("de-DE same file", () => AssertValid("de-DE"));
    Check("fr-FR same file", () => AssertValid("fr-FR"));
    Check("vi-VN same file", () => AssertValid("vi-VN"));
    Check("reject comma in en-US", () => AssertRejected("en-US", "ROLL-X;1,234"));
    Check("reject comma in de-DE", () => AssertRejected("de-DE", "ROLL-X;1,234"));
    Check("reject malformed number", () => AssertRejected("en-US", "ROLL-X;1.2.3"));
    Check("reject empty number", () => AssertRejected("en-US", "ROLL-X;"));
    Check("reject missing roll ID", () => AssertRejected("en-US", ";1.234"));
    return new ContractReport(passed, failed, controlPassed, messages);
}

internal sealed record ContractReport(
    int Passed, int Failed, bool ControlPassed, IReadOnlyList<string> Messages);
