var store = new ProfileStore(new Profile("ha", "Ha", "Hanoi", 1));
var clientA = store.Get();
var clientB = store.Get();

Console.WriteLine(store.Update(clientA with { City = "Da Nang" }, $"\"{clientA.Version}\""));
Console.WriteLine(store.Update(clientB with { DisplayName = "Ha Nguyen" }, $"\"{clientB.Version}\""));
Console.WriteLine(store.Get());

public sealed record Profile(string Id, string DisplayName, string City, long Version);

public sealed class ProfileStore
{
    private Profile _profile;
    public ProfileStore(Profile profile) => _profile = profile;
    public Profile Get() => _profile;

    public string Update(Profile requested, string ifMatch)
    {
        var currentEtag = $"\"{_profile.Version}\"";
        if (!StringComparer.Ordinal.Equals(ifMatch, currentEtag))
            return "412 Precondition Failed";

        _profile = requested with { Version = _profile.Version + 1 };
        return "200 OK";
    }
}
