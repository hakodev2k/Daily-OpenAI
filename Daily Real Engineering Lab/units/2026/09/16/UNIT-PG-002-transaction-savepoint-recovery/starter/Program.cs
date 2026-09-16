var tx = new PgTransactionSimulator();
var rows = new[] { "A", "DUPLICATE", "B" };

foreach (var row in rows)
{
    try
    {
        tx.Insert(row);
        Console.WriteLine($"IMPORTED {row}");
    }
    catch (ConstraintViolationException ex)
    {
        Console.WriteLine($"SKIPPED {row}: {ex.Message}");
        // Business allows this row to be skipped. What transaction state
        // should the next iteration expect at this point?
    }
}

try
{
    tx.Commit();
    Console.WriteLine("COMMIT OK");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"COMMIT FAILED: {ex.Message}");
}

Console.WriteLine($"PERSISTED {string.Join(',', tx.Persisted)}");

sealed class PgTransactionSimulator
{
    private readonly List<string> _pending = [];
    private List<string>? _savepoint;
    private bool _aborted;
    public IReadOnlyList<string> Persisted { get; private set; } = Array.Empty<string>();

    public void CreateSavepoint() => _savepoint = [.. _pending];

    public void RollbackToSavepoint()
    {
        if (_savepoint is null) throw new InvalidOperationException("No savepoint exists.");
        _pending.Clear();
        _pending.AddRange(_savepoint);
        _aborted = false;
    }

    public void Insert(string value)
    {
        if (_aborted) throw new InvalidOperationException("current transaction is aborted, commands ignored until end of transaction block");
        if (value == "DUPLICATE")
        {
            _aborted = true;
            throw new ConstraintViolationException("duplicate key value violates unique constraint");
        }
        _pending.Add(value);
    }

    public void Commit()
    {
        if (_aborted) throw new InvalidOperationException("cannot commit while transaction is aborted");
        Persisted = _pending.ToArray();
    }
}

sealed class ConstraintViolationException(string message) : Exception(message);