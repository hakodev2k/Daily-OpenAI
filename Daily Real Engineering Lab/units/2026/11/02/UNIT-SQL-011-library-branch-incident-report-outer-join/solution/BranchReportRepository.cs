using Microsoft.Data.Sqlite;

namespace EngineeringLab;

public sealed record BranchSummary(int BranchId, string BranchName, long OpenCriticalCount);

public sealed class BranchReportRepository
{
    public IReadOnlyList<BranchSummary> Load(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT b.Id, b.Name, COUNT(i.Id) AS OpenCriticalCount
            FROM Branches AS b
            LEFT JOIN Incidents AS i
                ON i.BranchId = b.Id
               AND i.Status = 'Open'
               AND i.Severity = 'Critical'
            GROUP BY b.Id, b.Name
            ORDER BY b.Id;
            """;

        using var reader = command.ExecuteReader();
        var results = new List<BranchSummary>();
        while (reader.Read())
        {
            results.Add(new BranchSummary(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetInt64(2)));
        }

        return results;
    }
}
