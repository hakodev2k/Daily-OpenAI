using System.Globalization;
using System.Text.Json;

var events = new[]
{
    """{"eventId":9007199254740992,"parcel":"P-101","status":"Loaded"}""",
    """{"eventId":9007199254740993,"parcel":"P-102","status":"Loaded"}""",
    """{"eventId":9007199254740994,"parcel":"P-103","status":"Loaded"}"""
};
var accepted = new Dictionary<string,string>(StringComparer.Ordinal);
foreach (var payload in events)
{
    using var doc = JsonDocument.Parse(payload);
    var rawId = doc.RootElement.GetProperty("eventId");
    var key = rawId.GetInt64().ToString(CultureInfo.InvariantCulture);
    var parcel = doc.RootElement.GetProperty("parcel").GetString()!;
    if (!accepted.TryAdd(key, parcel))
        Console.WriteLine($"DUPLICATE key={key}, parcel={parcel}");
    else
        Console.WriteLine($"ACCEPT key={key}, parcel={parcel}");
}
Console.WriteLine($"ACCEPTED={accepted.Count}; EXPECTED=3");
return accepted.Count == 3 ? 0 : 1;
