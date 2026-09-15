var store = new ProfileStore(new Profile("ha", "Ha", "Hanoi", 1));

var clientA = store.Get();
var clientB = store.Get();
Console.WriteLine($"A read: {clientA}");
Console.WriteLine($"B read: {clientB}");

var aResult = store.Update(clientA with { City = "Da Nang" });
Console.WriteLine($"A write: {aResult}; state={store.Get()}");

var bResult = store.Update(clientB with { DisplayName = "Ha Nguyen" });
Console.WriteLine($"B stale write: {bResult}; state={store.Get()}");

public sealed record Profile(string Id, string DisplayName, string City, long Version);

public sealed class ProfileStore
{
    private Profile _profile;
    public ProfileStore(Profile profile) => _profile = profile;
    public Profile Get() => _profile;

    // TODO: Điều tra contract. Version mà client đã đọc hiện chưa tham gia quyết định write.
    public string Update(Profile requested)
    {
        _profile = requested with { Version = _profile.Version + 1 };
        return "200 OK";
    }
}
