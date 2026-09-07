using Microsoft.Data.Sqlite;

await using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();

var setup = connection.CreateCommand();
setup.CommandText = """
CREATE TABLE Orders (
    Id INTEGER PRIMARY KEY,
    CreatedUtc TEXT NOT NULL,
    Total REAL NOT NULL
);
CREATE INDEX IX_Orders_CreatedUtc ON Orders(CreatedUtc);
""";
await setup.ExecuteNonQueryAsync();

await using (var tx = await connection.BeginTransactionAsync())
{
    var insert = connection.CreateCommand();
    insert.Transaction = (SqliteTransaction)tx;
    insert.CommandText = "INSERT INTO Orders (Id, CreatedUtc, Total) VALUES ($id, $created, $total)";
    var id = insert.Parameters.Add("$id", SqliteType.Integer);
    var created = insert.Parameters.Add("$created", SqliteType.Text);
    var total = insert.Parameters.Add("$total", SqliteType.Real);

    var start = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc);
    for (var i = 1; i <= 20000; i++)
    {
        id.Value = i;
        created.Value = start.AddMinutes(i).ToString("yyyy-MM-dd HH:mm:ss");
        total.Value = i % 500;
        await insert.ExecuteNonQueryAsync();
    }
    await tx.CommitAsync();
}

const string targetDay = "2026-09-07";

// Learner-editable query. Preserve the same result semantics.
var sql = "SELECT COUNT(*) FROM Orders WHERE date(CreatedUtc) = $day";

var plan = connection.CreateCommand();
plan.CommandText = "EXPLAIN QUERY PLAN " + sql;
plan.Parameters.AddWithValue("$day", targetDay);
await using (var reader = await plan.ExecuteReaderAsync())
{
    while (await reader.ReadAsync())
        Console.WriteLine($"Plan={reader.GetString(3)}");
}

var query = connection.CreateCommand();
query.CommandText = sql;
query.Parameters.AddWithValue("$day", targetDay);
var count = Convert.ToInt32(await query.ExecuteScalarAsync());
Console.WriteLine($"Count={count}");
