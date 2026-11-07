namespace SeedRegistryLab;

// Reference implementation. Replace starter/AccessionRegistry.cs to compare.
public sealed class AccessionRegistry
{
    private readonly Dictionary<string, SeedAccession> _byCode =
        new(StringComparer.Ordinal);

    public int Count => _byCode.Count;

    public bool Register(string code)
    {
        Validate(code);
        return _byCode.TryAdd(code, new SeedAccession(code));
    }

    public bool CorrectCode(string oldCode, string newCode)
    {
        Validate(oldCode);
        Validate(newCode);
        if (!_byCode.TryGetValue(oldCode, out var item)) return false;
        if (StringComparer.Ordinal.Equals(oldCode, newCode)) return true;
        if (_byCode.ContainsKey(newCode)) return false;

        // The index key is immutable; the record is reindexed on correction.
        _byCode.Remove(oldCode);
        item.Code = newCode;
        _byCode.Add(newCode, item);
        return true;
    }

    public bool ContainsCode(string code)
    {
        Validate(code);
        return _byCode.ContainsKey(code);
    }

    public bool RemoveCode(string code)
    {
        Validate(code);
        return _byCode.Remove(code);
    }

    private static void Validate(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Accession code is required.", nameof(code));
    }
}
