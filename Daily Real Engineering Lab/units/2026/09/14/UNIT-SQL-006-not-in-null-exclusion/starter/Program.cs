using Microsoft.Data.Sqlite;

using var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();

using (var setup = connection.CreateCommand())
{
    setup.CommandText = """
        CREATE TABLE Customers (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL);
        CREATE TABLE BlockedCustomers (CustomerId INTEGER NULL, Reason TEXT NOT NULL);

        INSERT INTO Customers (Id, Name) VALUES
            (1, 'An'),
            (2, 'Binh'),
            (3, 'Chi'),
            (4, 'Dung');

        INSERT INTO BlockedCustomers (CustomerId, Reason) VALUES
            (2, 'manual-review'),
            (NULL, 'legacy-import-row');
        """;
    setup.ExecuteNonQuery();
}

var customers = ReadIds(connection, "SELECT Id FROM Customers ORDER BY Id;");
var blocked = ReadNullableIds(connection, "SELECT CustomerId FROM BlockedCustomers ORDER BY rowid;");

// Investigation target: the application expects every customer not explicitly blocked.
const string eligibilitySql = """
    SELECT Id
    FROM Customers
    WHERE Id NOT IN (SELECT CustomerId FROM BlockedCustomers)
    ORDER BY Id;
    """;

var actual = ReadIds(connection, eligibilitySql);
var expected = new[] { 1, 3, 4 };

Console.WriteLine($"CUSTOMERS={string.Join(',', customers)}");
Console.WriteLine($"BLOCKED={string.Join(',', blocked.Select(x => x?.ToString() ?? "NULL"))}");
Console.WriteLine($"EXPECTED={string.Join(',', expected)}");
Console.WriteLine($"ACTUAL={string.Join(',', actual)}");

if (actual.SequenceEqual(expected))
{
    Console.WriteLine("RESULT=PASS");
    return 0;
}

Console.WriteLine("RESULT=FAIL");
return 2;

static List<int> ReadIds(SqliteConnection connection, string sql)
{
    using var command = connection.CreateCommand();
    command.CommandText = sql;
    using var reader = command.ExecuteReader();

    var result = new List<int>();
    while (reader.Read())
    {
        result.Add(reader.GetInt32(0));
    }

    return result;
}

static List<int?> ReadNullableIds(SqliteConnection connection, string sql)
{
    using var command = connection.CreateCommand();
    command.CommandText = sql;
    using var reader = command.ExecuteReader();

    var result = new List<int?>();
    while (reader.Read())
    {
        result.Add(reader.IsDBNull(0) ? null : reader.GetInt32(0));
    }

    return result;
}
