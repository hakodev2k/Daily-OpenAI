using System.Globalization;

public sealed class RevisionedChecklistStore
{
    private readonly IConcurrencyProbe _probe;
    private string _note = "Inspect park lights";
    private int _revision = 7;

    public RevisionedChecklistStore(IConcurrencyProbe probe) => _probe = probe;

    public ChecklistSnapshot Read() => new(_note, _revision);

    public async Task<PatchResult> TryPatchAsync(string? ifMatch, string newNote)
    {
        if (ifMatch is null) return new(428, Quote(_revision));
        if (!ParseStrongTag(ifMatch, out var expected)) return new(400, Quote(_revision));
        if (expected != _revision) return new(412, Quote(_revision));

        await _probe.PauseAsync();
        _note = newNote;
        _revision = expected + 1;
        return new(200, Quote(_revision));
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
