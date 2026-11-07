using Microsoft.Data.Sqlite;

var dbPath = Path.Combine(AppContext.BaseDirectory, "inventory.db");
if (File.Exists(dbPath)) File.Delete(dbPath);

var cs = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();

await using (var setup = new SqliteConnection(cs))
{
    await setup.OpenAsync();
    var cmd = setup.CreateCommand();
    cmd.CommandText = "CREATE TABLE Inventory (Sku TEXT PRIMARY KEY, Quantity INTEGER NOT NULL); INSERT INTO Inventory VALUES ('SKU-1', 10);";
    await cmd.ExecuteNonQueryAsync();
}

async Task<bool> ReserveAsync(string name, int amount)
{
    await using var connection = new SqliteConnection(cs);
    await connection.OpenAsync();

    var read = connection.CreateCommand();
    read.CommandText = "SELECT Quantity FROM Inventory WHERE Sku = 'SKU-1';";
    var quantity = Convert.ToInt32(await read.ExecuteScalarAsync());
    Console.WriteLine($"{name}: READ quantity={quantity}");

    if (quantity < amount)
    {
        Console.WriteLine($"{name}: REJECTED");
        return false;
    }

    await Task.Delay(150);

    var write = connection.CreateCommand();
    write.CommandText = "UPDATE Inventory SET Quantity = $newQuantity WHERE Sku = 'SKU-1';";
    write.Parameters.AddWithValue("$newQuantity", quantity - amount);
    await write.ExecuteNonQueryAsync();

    Console.WriteLine($"{name}: RESERVED amount={amount}");
    return true;
}

var results = await Task.WhenAll(
    ReserveAsync("reservation-A", 7),
    ReserveAsync("reservation-B", 7));

await using var verify = new SqliteConnection(cs);
await verify.OpenAsync();
var final = verify.CreateCommand();
final.CommandText = "SELECT Quantity FROM Inventory WHERE Sku = 'SKU-1';";
var finalQuantity = Convert.ToInt32(await final.ExecuteScalarAsync());

Console.WriteLine($"SUCCESS_COUNT={results.Count(x => x)}");
Console.WriteLine($"FINAL_QUANTITY={finalQuantity}");
