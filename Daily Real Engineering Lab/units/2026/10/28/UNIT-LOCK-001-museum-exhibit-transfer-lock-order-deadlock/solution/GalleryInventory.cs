namespace ExhibitLedger;

public sealed class GalleryInventory
{
    private sealed class Gallery
    {
        public Gallery(string code, int quantity)
        {
            Code = code;
            Quantity = quantity;
        }

        public string Code { get; }
        public object SyncRoot { get; } = new();
        public int Quantity { get; set; }
    }

    private readonly Dictionary<string, Gallery> _galleries =
        new(StringComparer.Ordinal)
        {
            ["NORTH"] = new Gallery("NORTH", 10),
            ["SOUTH"] = new Gallery("SOUTH", 10)
        };

    public void Transfer(string sourceCode, string destinationCode, int quantity,
        Action afterFirstResourceAcquired)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));
        if (string.Equals(sourceCode, destinationCode, StringComparison.Ordinal))
            throw new ArgumentException("Source and destination must differ.");

        var source = _galleries[sourceCode];
        var destination = _galleries[destinationCode];

        // Every two-resource operation follows the same total lock order.
        var first = string.CompareOrdinal(source.Code, destination.Code) < 0
            ? source : destination;
        var second = ReferenceEquals(first, source) ? destination : source;

        lock (first.SyncRoot)
        {
            afterFirstResourceAcquired();

            lock (second.SyncRoot)
            {
                if (source.Quantity < quantity)
                    throw new InvalidOperationException("Insufficient stock.");

                source.Quantity -= quantity;
                destination.Quantity += quantity;
            }
        }
    }

    public (int North, int South) Snapshot()
    {
        lock (_galleries["NORTH"].SyncRoot)
        lock (_galleries["SOUTH"].SyncRoot)
        {
            return (_galleries["NORTH"].Quantity, _galleries["SOUTH"].Quantity);
        }
    }
}
