using System.Collections.Concurrent;

var mode = args.FirstOrDefault() ?? "verify";

var store = new FakeBlobStore();
await store.SeedAsync("article.md", "Original");
var service = new DocumentService(store);

var editorA = await service.OpenAsync("article.md");
var editorB = await service.OpenAsync("article.md");
var saveA = editorA with { Content = "Edit from A" };
var saveB = editorB with { Content = "Edit from B" };

await service.SaveAsync(saveA);

var staleWriteRejected = false;
try
{
    await service.SaveAsync(saveB);
}
catch (PreconditionFailedException ex)
{
    staleWriteRejected = true;
    Console.WriteLine($"Save B rejected: {ex.Message}");
}

var final = await service.OpenAsync("article.md");
var preserved = final.Content == "Edit from A";
Console.WriteLine($"Final content: {final.Content}");
Console.WriteLine(staleWriteRejected && preserved ? "REFERENCE_PASS" : "REFERENCE_FAIL");
return staleWriteRejected && preserved ? 0 : 1;

public sealed record DocumentSnapshot(string Name, string Content, string ETag);

public sealed class DocumentService(FakeBlobStore store)
{
    public Task<DocumentSnapshot> OpenAsync(string name) => store.GetAsync(name);

    public Task SaveAsync(DocumentSnapshot edited)
        => store.PutAsync(edited.Name, edited.Content, expectedETag: edited.ETag);
}

public sealed class FakeBlobStore
{
    private readonly ConcurrentDictionary<string, BlobState> _blobs = new();

    public Task SeedAsync(string name, string content)
    {
        _blobs[name] = new BlobState(content, 1);
        return Task.CompletedTask;
    }

    public Task<DocumentSnapshot> GetAsync(string name)
    {
        if (!_blobs.TryGetValue(name, out var state))
            throw new FileNotFoundException(name);

        return Task.FromResult(new DocumentSnapshot(name, state.Content, ToETag(state.Version)));
    }

    public Task PutAsync(string name, string content, string? expectedETag)
    {
        while (true)
        {
            if (!_blobs.TryGetValue(name, out var current))
                throw new FileNotFoundException(name);

            if (expectedETag is not null && !StringComparer.Ordinal.Equals(expectedETag, ToETag(current.Version)))
                throw new PreconditionFailedException($"Expected {expectedETag}, current {ToETag(current.Version)}");

            var next = new BlobState(content, current.Version + 1);
            if (_blobs.TryUpdate(name, next, current))
                return Task.CompletedTask;
        }
    }

    private static string ToETag(long version) => $"\"v{version}\"";
    private sealed record BlobState(string Content, long Version);
}

public sealed class PreconditionFailedException(string message) : Exception(message);
