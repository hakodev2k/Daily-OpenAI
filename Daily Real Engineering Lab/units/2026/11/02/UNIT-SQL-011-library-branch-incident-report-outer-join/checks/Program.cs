using EngineeringLab;
using Microsoft.Data.Sqlite;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length != 1 || !new[] { "inspect", "reproduce", "verify" }.Contains(args[0]))
        {
            Console.Error.WriteLine("Usage: dotnet run --project checks/Checks.csproj -- inspect|reproduce|verify");
            return 2;
        }

        try
        {
            using var db = OpenSeededDatabase();
            var repository = new BranchReportRepository();

            switch (args[0])
            {
                case "inspect":
                    Print(repository.Load(db));
                    Console.WriteLine("Database branch count: " + Scalar(db, "SELECT COUNT(*) FROM Branches"));
                    Console.WriteLine("Database open-critical count: " + Scalar(db,
                        "SELECT COUNT(*) FROM Incidents WHERE Status = 'Open' AND Severity = 'Critical'"));
                    break;
                case "reproduce":
                    Reproduce(db, repository);
                    break;
                case "verify":
                    Verify(db, repository);
                    break;
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("FAILED: " + ex);
            return 1;
        }
    }

    private static SqliteConnection OpenSeededDatabase()
    {
        var db = new SqliteConnection("Data Source=:memory:");
        db.Open();
        var statements = new[]
        {
            "PRAGMA foreign_keys = ON",
            "CREATE TABLE Branches (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL)",
            "CREATE TABLE Incidents (Id INTEGER PRIMARY KEY, BranchId INTEGER NOT NULL REFERENCES Branches(Id), Status TEXT NOT NULL, Severity TEXT NOT NULL)",
            "INSERT INTO Branches(Id, Name) VALUES (1, 'Central'), (2, 'West'), (3, 'North'), (4, 'East')",
            "INSERT INTO Incidents(Id, BranchId, Status, Severity) VALUES (101,1,'Open','Critical'), (102,1,'Open','Critical'), (103,1,'Closed','Critical'), (104,1,'Open','Minor'), (105,2,'Open','Minor'), (106,4,'Closed','Critical')"
        };
        foreach (var sql in statements) Execute(db, sql);
        return db;
    }

    private static void Reproduce(SqliteConnection db, BranchReportRepository repository)
    {
        var rows = repository.Load(db);
        Print(rows);
        var branches = Scalar(db, "SELECT COUNT(*) FROM Branches");
        var critical = Scalar(db,
            "SELECT COUNT(*) FROM Incidents WHERE Status = 'Open' AND Severity = 'Critical'");
        if (branches != 4 || critical != 2 || rows.Count != 1 ||
            rows[0].BranchId != 1 || rows[0].OpenCriticalCount != 2)
            throw new InvalidOperationException("Expected starter symptom was not reproduced.");
        Console.WriteLine("REPRODUCED: four branches exist, but only one appears; incident total remains two.");
    }

    private static void Verify(SqliteConnection db, BranchReportRepository repository)
    {
        AssertRows(repository.Load(db), new Dictionary<int, long>
        {
            [1] = 2, [2] = 0, [3] = 0, [4] = 0
        }, "initial");

        Execute(db, "INSERT INTO Branches(Id, Name) VALUES (5, 'South')");
        Execute(db, "INSERT INTO Incidents(Id, BranchId, Status, Severity) VALUES (107,5,'Closed','Critical')");
        AssertRows(repository.Load(db), new Dictionary<int, long>
        {
            [1] = 2, [2] = 0, [3] = 0, [4] = 0, [5] = 0
        }, "closed-only new branch");

        Execute(db, "INSERT INTO Incidents(Id, BranchId, Status, Severity) VALUES (108,2,'Open','Critical')");
        AssertRows(repository.Load(db), new Dictionary<int, long>
        {
            [1] = 2, [2] = 1, [3] = 0, [4] = 0, [5] = 0
        }, "new open-critical incident");

        Console.WriteLine("VERIFIED: all branches retained; counts, zero groups and changes match contract.");
    }

    private static void AssertRows(
        IReadOnlyList<BranchSummary> actual,
        IReadOnlyDictionary<int, long> expected,
        string stage)
    {
        var ids = actual.Select(x => x.BranchId).ToArray();
        if (actual.Count != expected.Count ||
            !ids.SequenceEqual(expected.Keys.OrderBy(x => x)))
            throw new InvalidOperationException(stage + ": missing, duplicated or unordered branches.");

        foreach (var row in actual)
        {
            if (!expected.TryGetValue(row.BranchId, out var count) ||
                row.OpenCriticalCount != count)
                throw new InvalidOperationException(stage + ": incorrect count for branch " + row.BranchId);
        }

        var total = actual.Sum(x => x.OpenCriticalCount);
        if (total != expected.Values.Sum())
            throw new InvalidOperationException(stage + ": inconsistent city-wide total.");
    }

    private static void Execute(SqliteConnection db, string sql)
    {
        using var command = db.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static long Scalar(SqliteConnection db, string sql)
    {
        using var command = db.CreateCommand();
        command.CommandText = sql;
        return Convert.ToInt64(command.ExecuteScalar());
    }

    private static void Print(IReadOnlyList<BranchSummary> rows)
    {
        foreach (var row in rows)
            Console.WriteLine(row.BranchId + " | " + row.BranchName + " | " + row.OpenCriticalCount);
        Console.WriteLine("Report rows: " + rows.Count);
    }
}
