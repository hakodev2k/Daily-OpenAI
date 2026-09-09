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
    // Investigation note:
    // The query expresses an exclusion rule. Verify how every value produced
    // by the subquery participates in the predicate for a candidate row.
    command.CommandText = """
        SELECT Id
        FROM Customers
        WHERE IsActive = 1
          AND Id NOT IN (SELECT CustomerId FROM Suppression)
        ORDER BY Id;
        """;

    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        eligible.Add(reader.GetInt64(0));
    }
}

Console.WriteLine($"ACTIVE_CUSTOMERS=1,2,3");
Console.WriteLine($"SUPPRESSION_ROWS=2,NULL");
Console.WriteLine($"OBSERVED_ELIGIBLE_IDS={string.Join(',', eligible)}");
Console.WriteLine($"OBSERVED_ELIGIBLE_COUNT={eligible.Count}");

var expected = new long[] { 1, 3 };
var matchesBusinessRule = eligible.SequenceEqual(expected);
Console.WriteLine($"BUSINESS_RULE_MATCH={matchesBusinessRule}");

return matchesBusinessRule ? 0 : 10;
