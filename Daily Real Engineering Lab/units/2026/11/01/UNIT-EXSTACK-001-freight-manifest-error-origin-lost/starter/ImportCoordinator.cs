namespace FreightManifest;

public interface IImportAudit
{
    int RejectedCount { get; }
    void Rejected(string category);
}

public sealed class InMemoryImportAudit : IImportAudit
{
    private readonly List<string> _categories = new();
    public int RejectedCount => _categories.Count;
    public void Rejected(string category) => _categories.Add(category);
}

public sealed class ImportCoordinator
{
    private readonly SourceParser _parser;
    private readonly IImportAudit _audit;

    public ImportCoordinator(SourceParser parser, IImportAudit audit)
    {
        _parser = parser;
        _audit = audit;
    }

    public ManifestLine Import(string raw)
    {
        try
        {
            return _parser.Parse(raw);
        }
        catch (Exception error) when (error is FormatException or InvalidDataException)
        {
            _audit.Rejected(error.GetType().Name);
            throw error;
        }
    }
}
