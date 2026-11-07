using Npgsql;

const string connectionString = "Host=localhost;Port=55432;Username=lab;Password=lab;Database=engineering_lab";

await using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();

await using (var reset = new NpgsqlCommand("""
    DROP TABLE IF EXISTS customers;
    CREATE TABLE customers (
        id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        external_id text NOT NULL UNIQUE,
        name text NOT NULL
    );
    """, connection))
{
    await reset.ExecuteNonQueryAsync();
}

var batch = new[]
{
    new Customer("CUST-100", "An"),
    new Customer("CUST-100", "An duplicate"),
    new Customer("CUST-200", "Binh")
};

var inserted = 0;
var skipped = 0;

await using var transaction = await connection.BeginTransactionAsync();

foreach (var customer in batch)
{
    await using var command = new NpgsqlCommand("""
        INSERT INTO customers (external_id, name)
        VALUES (@externalId, @name)
        ON CONFLICT (external_id) DO NOTHING;
        """, connection, transaction);

    command.Parameters.AddWithValue("externalId", customer.ExternalId);
    command.Parameters.AddWithValue("name", customer.Name);

    var affected = await command.ExecuteNonQueryAsync();
    if (affected == 1)
    {
        inserted++;
        Console.WriteLine($"INSERT_OK externalId={customer.ExternalId}");
    }
    else
    {
        skipped++;
        Console.WriteLine($"DUPLICATE_SKIPPED externalId={customer.ExternalId}");
    }
}

await transaction.CommitAsync();

await using var countCommand = new NpgsqlCommand("SELECT COUNT(*) FROM customers;", connection);
var rowCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());

Console.WriteLine($"INSERTED_COMMAND_COUNT={inserted}");
Console.WriteLine($"DUPLICATE_SKIPPED_COUNT={skipped}");
Console.WriteLine($"ROW_COUNT={rowCount}");
Console.WriteLine("ABORTED_STATE_OBSERVED=false");
Console.WriteLine("BATCH_COMMITTED=true");

internal sealed record Customer(string ExternalId, string Name);
