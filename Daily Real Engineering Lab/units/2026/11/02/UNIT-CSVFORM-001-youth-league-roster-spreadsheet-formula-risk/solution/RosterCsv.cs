using System.Text;
namespace LeagueRoster;
public sealed record Registration(string Id, string Participant, string Team, string Comment);
public static class RosterCsvExporter
{
    public static string Export(IEnumerable<Registration> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id,Participant,Team,Comment");
        foreach (var r in rows)
            sb.Append(Quote(ForSpreadsheet(r.Id))).Append(',')
              .Append(Quote(ForSpreadsheet(r.Participant))).Append(',')
              .Append(Quote(ForSpreadsheet(r.Team))).Append(',')
              .Append(Quote(ForSpreadsheet(r.Comment))).Append('\n');
        return sb.ToString();
    }
    private static string ForSpreadsheet(string value)
    {
        if (value.Length == 0 || value[0] == '\'') return value;
        if (value[0] is '\t' or '\r' or '\n') return "'" + value;
        string trimmed = value.TrimStart(' ', '\t', '\r', '\n');
        return trimmed.Length > 0 && (trimmed[0] is '=' or '+' or '-' or '@')
            ? "'" + value : value;
    }
    private static string Quote(string s) => "\"" + s.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
}
