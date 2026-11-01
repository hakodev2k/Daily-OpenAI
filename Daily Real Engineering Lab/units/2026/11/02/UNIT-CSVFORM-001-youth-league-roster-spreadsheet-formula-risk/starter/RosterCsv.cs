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
            sb.Append(Quote(r.Id)).Append(',').Append(Quote(r.Participant)).Append(',')
              .Append(Quote(r.Team)).Append(',').Append(Quote(r.Comment)).Append('\n');
        return sb.ToString();
    }
    private static string Quote(string s) => "\"" + s.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
}
