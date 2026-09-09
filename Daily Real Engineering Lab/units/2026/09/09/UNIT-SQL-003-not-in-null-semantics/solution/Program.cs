using Microsoft.Data.Sqlite;

using var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();

using (var setup = connection.CreateCommand())
{
    setup.CommandText = """
        CREATE TABLE Customers (Id INTEGER PRIMARY KEY, IsActive INTEGER NOT NULL);
        CREATE TABLE Suppression (CustomerId INTEGER NULL);

        INSERT INTO Customers (Id, IsActive) VALUES
            (1, 1),
            (2, 1),
            (3, 1),
            (4, 0);

        INSERT INTO Suppression (CustomerId) VALUES
            (2),
            (NULL);
        """;
    setup.ExecuteNonQuery();
}

var eligible = new List<long>();
using (var command = connection.CreateCommand())
{
    command.CommandText = """
        SELECT c.Id
        FROM Customers AS c
        WHERE c.IsActive = 1
          AND NOT EXISTS (
              SELECT 1
              FROM Suppression AS s
              WHERE s.CustomerId = c.Id
          )
        ORDER BY c.Id;
        """;

    using var reader = command.ExecuteReader();
    while (reader.Read())
        eligible.Add(reader.GetInt64(0));
}

Console.WriteLine($"OBSERVED_ELIGIBLE_IDS={string.Join(',', eligible)}");
var expected = new long[] { 1, 3 };
var ok = eligible.SequenceEqual(expected);
Console.WriteLine($"BUSINESS_RULE_MATCH={ok}");
return ok ? 0 : 10;
