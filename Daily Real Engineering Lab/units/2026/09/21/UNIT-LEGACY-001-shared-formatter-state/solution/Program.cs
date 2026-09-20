using System.Text;

record Statement(string Id, string Customer, string[] Lines);

static class StatementFormatter
{
    public static string Render(Statement statement)
    {
        var buffer = new StringBuilder();
        buffer.AppendLine($"STATEMENT:{statement.Id}");
        buffer.AppendLine($"CUSTOMER:{statement.Customer}");
        foreach (var line in statement.Lines) buffer.AppendLine($"ITEM:{line}");
        return buffer.ToString();
    }
}

var a = new Statement("A-100", "An", ["Hosting", "Support"]);
var b = new Statement("B-200", "Binh", ["Consulting"]);
var results = await Task.WhenAll(Task.Run(() => StatementFormatter.Render(a)), Task.Run(() => StatementFormatter.Render(b)));
Console.WriteLine(results[0]);
Console.WriteLine(results[1]);