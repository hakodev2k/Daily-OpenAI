record Profile(string Id, string Address, bool MarketingEmail, int Version);

var store = new ProfileStore(new Profile("C-42", "Old Street", false, 1));
var addressRequest = store.Read();
var preferenceRequest = store.Read();

Console.WriteLine($"address-read v{addressRequest.Version}: {addressRequest.Address}, marketing={addressRequest.MarketingEmail}");
Console.WriteLine($"preference-read v{preferenceRequest.Version}: {preferenceRequest.Address}, marketing={preferenceRequest.MarketingEmail}");

var addressResult = store.Save(addressRequest with { Address = "New Avenue" });
Console.WriteLine($"address-save={addressResult}");
var preferenceResult = store.Save(preferenceRequest with { MarketingEmail = true });
Console.WriteLine($"preference-save={preferenceResult}");

var final = store.Read();
Console.WriteLine($"final v{final.Version}: {final.Address}, marketing={final.MarketingEmail}");

var lostChange = final.Address != "New Avenue" || final.MarketingEmail != true;
if (args.Contains("reproduce"))
{
    Console.WriteLine(lostChange ? "REPRODUCED" : "NOT_REPRODUCED");
    return lostChange ? 2 : 0;
}

if (args.Contains("verify"))
{
    var conflictSafelyRejected = addressResult && !preferenceResult && final.Address == "New Avenue";
    var bothChangesPreserved = addressResult && preferenceResult && final.Address == "New Avenue" && final.MarketingEmail;
    var acceptable = conflictSafelyRejected || bothChangesPreserved;
    Console.WriteLine(acceptable ? "VERIFY_PASS" : "VERIFY_FAIL");
    return acceptable ? 0 : 3;
}

return 0;

sealed class ProfileStore(Profile initial)
{
    private Profile _current = initial;
    public Profile Read() => _current with { };

    public bool Save(Profile candidate)
    {
        // Investigation note: what proves that the state read by this request
        // is still the state on which this write is allowed to operate?
        _current = candidate with { Version = _current.Version + 1 };
        return true;
    }
}