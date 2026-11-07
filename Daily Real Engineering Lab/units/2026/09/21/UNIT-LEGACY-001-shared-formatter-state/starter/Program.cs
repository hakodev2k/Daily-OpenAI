using System.Text;

record Statement(string Id, string Customer, string[] Lines);

static class LegacyStatementFormatter
{
    private static readonly StringBuilder Buffer = new();
    public static Action<string>? Checkpoint { get; set; }

    public static string Render(Statement statement)
    {
        Buffer.Clear();
        Buffer.AppendLine($"STATEMENT:{statement.Id}");
        Checkpoint?.Invoke(statement.Id);
        Buffer.AppendLine($"CUSTOMER:{statement.Customer}");
        foreach (var line in statement.Lines) Buffer.AppendLine($"ITEM:{line}");
        return Buffer.ToString();
    }
}

static bool IsValid(string text, Statement s) =>
    text.Contains($"STATEMENT:{s.Id}") && text.Contains($"CUSTOMER:{s.Customer}") &&
    s.Lines.All(x => text.Contains($"ITEM:{x}"));

var a = new Statement("A-100", "An", ["Hosting", "Support"]);
var b = new Statement("B-200", "Binh", ["Consulting"]);

if (args.Contains("--sequential"))
{
    var ok = IsValid(LegacyStatementFormatter.Render(a), a) && IsValid(LegacyStatementFormatter.Render(b), b);
    Console.WriteLine($"sequential={(ok ? "PASS" : "FAIL")}");
    return ok ? 0 : 2;
}

using var aPaused = new ManualResetEventSlim(false);
using var allowA = new ManualResetEventSlim(false);
LegacyStatementFormatter.Checkpoint = id =>
{
    if (id == a.Id) { aPaused.Set(); allowA.Wait(); }
};

var taskA = Task.Run(() => LegacyStatementFormatter.Render(a));
aPaused.Wait();
var textB = LegacyStatementFormatter.Render(b);
allowA.Set();
var textA = await taskA;
LegacyStatementFormatter.Checkpoint = null;

var parallelOk = IsValid(textA, a) && IsValid(textB, b) && !textA.Contains("B-200") && !textB.Contains("A-100");
Console.WriteLine($"parallel={(parallelOk ? "PASS" : "FAIL")}");
Console.WriteLine("A output:"); Console.WriteLine(textA);
Console.WriteLine("B output:"); Console.WriteLine(textB);

if (args.Contains("--reproduce")) return parallelOk ? 3 : 0;
if (args.Contains("--verify")) return parallelOk ? 0 : 4;
return parallelOk ? 0 : 1;