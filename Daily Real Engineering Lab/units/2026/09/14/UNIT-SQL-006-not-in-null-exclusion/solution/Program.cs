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

const string eligibilitySql = """
    SELECT c.Id
    FROM Customers AS c
    WHERE NOT EXISTS (
        SELECT 1
        FROM BlockedCustomers AS b
        WHERE b.CustomerId = c.Id
    )
    ORDER BY c.Id;
    """;

using var command = connection.CreateCommand();
command.CommandText = eligibilitySql;
using var reader = command.ExecuteReader();

var actual = new List<int>();
while (reader.Read())
{
    actual.Add(reader.GetInt32(0));
}

var expected = new[] { 1, 3, 4 };
Console.WriteLine($"EXPECTED={string.Join(',', expected)}");
Console.WriteLine($"ACTUAL={string.Join(',', actual)}");
Console.WriteLine(actual.SequenceEqual(expected) ? "RESULT=PASS" : "RESULT=FAIL");

return actual.SequenceEqual(expected) ? 0 : 2;
