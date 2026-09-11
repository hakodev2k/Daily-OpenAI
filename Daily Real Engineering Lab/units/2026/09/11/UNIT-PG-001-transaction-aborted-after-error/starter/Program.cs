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

var sawAbortedState = false;
var inserted = 0;

await using var transaction = await connection.BeginTransactionAsync();

foreach (var customer in batch)
{
    try
    {
        await using var command = new NpgsqlCommand(
            "INSERT INTO customers (external_id, name) VALUES (@externalId, @name);",
            connection,
            transaction);

        command.Parameters.AddWithValue("externalId", customer.ExternalId);
        command.Parameters.AddWithValue("name", customer.Name);

        inserted += await command.ExecuteNonQueryAsync();
        Console.WriteLine($"INSERT_OK externalId={customer.ExternalId}");
    }
    catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
    {
        Console.WriteLine($"DUPLICATE_CAUGHT sqlstate={ex.SqlState} externalId={customer.ExternalId}");
        // Business policy: duplicates are expected and should be skipped.
    }
    catch (PostgresException ex)
    {
        Console.WriteLine($"COMMAND_FAILED sqlstate={ex.SqlState} externalId={customer.ExternalId}");
        if (ex.SqlState == PostgresErrorCodes.InFailedSqlTransaction)
        {
            sawAbortedState = true;
        }
    }
}

try
{
    await transaction.CommitAsync();
    Console.WriteLine("COMMIT_ATTEMPTED=true");
}
catch (PostgresException ex)
{
    Console.WriteLine($"COMMIT_FAILED sqlstate={ex.SqlState}");
}

await using var countCommand = new NpgsqlCommand("SELECT COUNT(*) FROM customers;", connection);
var rowCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());

Console.WriteLine($"INSERTED_COMMAND_COUNT={inserted}");
Console.WriteLine($"ROW_COUNT={rowCount}");
Console.WriteLine($"ABORTED_STATE_OBSERVED={sawAbortedState.ToString().ToLowerInvariant()}");
Console.WriteLine("BATCH_COMMITTED=false");

internal sealed record Customer(string ExternalId, string Name);
