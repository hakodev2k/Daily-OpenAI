namespace SeedRegistryLab;

// Learner-editable: keep all public method signatures unchanged.
public sealed class AccessionRegistry
{
    private readonly HashSet<SeedAccession> _items = new();

    public int Count => _items.Count;

    public bool Register(string code)
    {
        Validate(code);
        return _items.Add(new SeedAccession(code));
    }

    public bool CorrectCode(string oldCode, string newCode)
    {
        Validate(oldCode);
        Validate(newCode);
        var accession = _items.FirstOrDefault(x => x.Code == oldCode);
        if (accession is null) return false;
        if (_items.Any(x => x.Code == newCode && !ReferenceEquals(x, accession))) return false;
        accession.Code = newCode;
        return true;
    }

    public bool ContainsCode(string code)
    {
        Validate(code);
        return _items.Contains(new SeedAccession(code));
    }

    public bool RemoveCode(string code)
    {
        Validate(code);
        return _items.Remove(new SeedAccession(code));
    }

    private static void Validate(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Accession code is required.", nameof(code));
    }
}
