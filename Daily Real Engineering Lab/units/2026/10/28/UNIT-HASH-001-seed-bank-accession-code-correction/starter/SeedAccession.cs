namespace SeedRegistryLab;

// Domain object received from a legacy import component.
public sealed class SeedAccession : IEquatable<SeedAccession>
{
    public SeedAccession(string code) => Code = code;
    public string Code { get; set; }

    public bool Equals(SeedAccession? other) =>
        other is not null && StringComparer.Ordinal.Equals(Code, other.Code);

    public override bool Equals(object? obj) => Equals(obj as SeedAccession);
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Code);
    public override string ToString() => Code;
}
