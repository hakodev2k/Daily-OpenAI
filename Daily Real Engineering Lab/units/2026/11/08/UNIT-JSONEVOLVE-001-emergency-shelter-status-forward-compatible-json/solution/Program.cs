using System.Text.Json;

var json = """
[
  {"id":"S1","status":"Available"},
  {"id":"S2","status":"Standby"},
  {"id":"S3","status":"Full"}
]
""";
using var document = JsonDocument.Parse(json);
var imported = new List<ShelterUpdate>();
foreach (var element in document.RootElement.EnumerateArray())
{
    var id = element.GetProperty("id").GetString() ?? throw new FormatException("Missing id");
    var raw = element.GetProperty("status").GetString() ?? throw new FormatException("Missing status");
    var status = raw switch
    {
        "Available" => ShelterStatus.Available,
        "Full" => ShelterStatus.Full,
        "Closed" => ShelterStatus.Closed,
        _ => ShelterStatus.Unknown
    };
    imported.Add(new ShelterUpdate(id, status, raw));
}
foreach (var shelter in imported)
    Console.WriteLine($"SHELTER={shelter.Id};STATUS={shelter.Status}");
Console.WriteLine($"IMPORTED={imported.Count}");
var review = imported.Where(s => s.Status == ShelterStatus.Unknown).ToArray();
Console.WriteLine($"REVIEW={review.Length}");
foreach (var item in review) Console.WriteLine($"RAW={item.Id}:{item.RawStatus}");

public enum ShelterStatus { Unknown, Available, Full, Closed }
public sealed record ShelterUpdate(string Id, ShelterStatus Status, string RawStatus);
