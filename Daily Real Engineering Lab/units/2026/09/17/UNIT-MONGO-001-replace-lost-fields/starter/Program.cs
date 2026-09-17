using System.Text.Json;

var store = new DocumentStore();
store.Seed(new Profile("u-42", "Ha", new Preferences("vi-VN", true), new Audit("import-v1")));

Console.WriteLine("BEFORE=" + JsonSerializer.Serialize(store.Get("u-42")));
var request = new RenameRequest("u-42", "Ha Nguyen");
ProfileUpdater.Apply(store, request);
Console.WriteLine("AFTER=" + JsonSerializer.Serialize(store.Get("u-42")));

public record RenameRequest(string Id, string DisplayName);
public record Preferences(string Locale, bool EmailEnabled);
public record Audit(string CreatedBy);
public record Profile(string Id, string DisplayName, Preferences? Preferences, Audit? Audit);

public sealed class DocumentStore
{
    private readonly Dictionary<string, Profile> _documents = new();
    public void Seed(Profile profile) => _documents[profile.Id] = profile;
    public Profile Get(string id) => _documents[id];
    public void Replace(Profile profile) => _documents[profile.Id] = profile;
    public void SetDisplayName(string id, string displayName)
    {
        var current = _documents[id];
        _documents[id] = current with { DisplayName = displayName };
    }
}

public static class ProfileUpdater
{
    public static void Apply(DocumentStore store, RenameRequest request)
    {
        // Investigation note: compare the write's scope with the request's intent.
        var replacement = new Profile(request.Id, request.DisplayName, null, null);
        store.Replace(replacement);
    }
}