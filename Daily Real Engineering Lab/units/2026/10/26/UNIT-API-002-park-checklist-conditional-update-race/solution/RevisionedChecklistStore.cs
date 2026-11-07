using System.Globalization;

public sealed class RevisionedChecklistStore
{
    private readonly IConcurrencyProbe _probe;
    private readonly object _sync = new();
    private string _note = "Inspect park lights";
    private int _revision = 7;

    public RevisionedChecklistStore(IConcurrencyProbe probe) => _probe = probe;

    public ChecklistSnapshot Read()
    {
        lock (_sync) return new(_note, _revision);
    }

    public async Task<PatchResult> TryPatchAsync(string? ifMatch, string newNote)
    {
        if (ifMatch is null) return new(428, ReadTag());
        if (!ParseStrongTag(ifMatch, out var expected)) return new(400, ReadTag());

        // Test hook: both requests have arrived before the atomic operation.
        await _probe.PauseAsync();
        lock (_sync)
        {
            if (expected != _revision) return new(412, Quote(_revision));
            _note = newNote;
            _revision++;
            return new(200, Quote(_revision));
        }
    }

    private string ReadTag()
    {
        lock (_sync) return Quote(_revision);
    }

    private static string Quote(int revision) => $"\"{revision}\"";

    private static bool ParseStrongTag(string tag, out int revision)
    {
        revision = default;
        return tag.Length >= 3 && tag[0] == '"' && tag[^1] == '"'
            && int.TryParse(tag[1..^1], NumberStyles.None,
                CultureInfo.InvariantCulture, out revision);
    }
}
