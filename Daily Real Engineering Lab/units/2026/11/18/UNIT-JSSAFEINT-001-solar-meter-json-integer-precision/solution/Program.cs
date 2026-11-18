using System.Globalization;
using System.Text.Json;

record Meter(long Id, decimal Kwh);
record WireMeter(string Id, decimal Kwh);

var meters = new[] { new Meter(9007199254740992L, 12.5m), new Meter(9007199254740993L, 13.5m) };
var payload = JsonSerializer.Serialize(meters.Select(x => new WireMeter(x.Id.ToString(CultureInfo.InvariantCulture), x.Kwh)));
using var document = JsonDocument.Parse(payload);
var clientIds = document.RootElement.EnumerateArray().Select(x => x.GetProperty("Id").GetString()!).ToArray();
Console.WriteLine("WIRE=" + payload);
Console.WriteLine("SERVER_IDS=" + string.Join(",", meters.Select(x => x.Id.ToString(CultureInfo.InvariantCulture))));
Console.WriteLine("CLIENT_IDS=" + string.Join(",", clientIds));
Console.WriteLine("DISTINCT_CLIENT_IDS=" + clientIds.Distinct(StringComparer.Ordinal).Count());
Console.WriteLine("ID_ROUNDTRIP_OK=" + (clientIds.Length == meters.Length && clientIds.Zip(meters).All(p => p.First == p.Second.Id.ToString(CultureInfo.InvariantCulture))).ToString().ToLowerInvariant());
Console.WriteLine("KWH_NUMERIC=" + document.RootElement.EnumerateArray().All(x => x.GetProperty("Kwh").ValueKind == JsonValueKind.Number).ToString().ToLowerInvariant());

