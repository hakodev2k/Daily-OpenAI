using Microsoft.Data.Sqlite;

using var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();
using (var setup = connection.CreateCommand())
{
    setup.CommandText = """
        CREATE TABLE Warehouses (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL);
        CREATE TABLE InventorySnapshots (Id INTEGER PRIMARY KEY, WarehouseId INTEGER NOT NULL, Quantity INTEGER NOT NULL, IsActive INTEGER NOT NULL);
        INSERT INTO Warehouses (Id, Name) VALUES (1, 'Ha Noi'), (2, 'Da Nang'), (3, 'Ho Chi Minh City');
        INSERT INTO InventorySnapshots (Id, WarehouseId, Quantity, IsActive) VALUES (101, 1, 25, 1), (102, 2, 8, 0);
        """;
    setup.ExecuteNonQuery();
}

const string reportSql = """
    SELECT w.Id, s.Quantity
    FROM Warehouses AS w
    LEFT JOIN InventorySnapshots AS s
        ON s.WarehouseId = w.Id
       AND s.IsActive = 1
    ORDER BY w.Id;
    """;

using var command = connection.CreateCommand();
command.CommandText = reportSql;
using var reader = command.ExecuteReader();
var rows = new List<(int WarehouseId, int? Quantity)>();
while (reader.Read()) rows.Add((reader.GetInt32(0), reader.IsDBNull(1) ? null : reader.GetInt32(1)));
Console.WriteLine($"ROWS={string.Join(';', rows.Select(x => $"{x.WarehouseId}:{x.Quantity?.ToString() ?? "NULL"}"))}");
var correct = rows.Count == 3 && rows[0] == (1, 25) && rows[1] == (2, null) && rows[2] == (3, null);
Console.WriteLine(correct ? "RESULT=PASS" : "RESULT=FAIL");
return correct ? 0 : 2;
