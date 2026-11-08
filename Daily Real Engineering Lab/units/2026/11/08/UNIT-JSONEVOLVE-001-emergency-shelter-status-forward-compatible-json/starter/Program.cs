using System.Text.Json;
using System.Text.Json.Serialization;

var json = """
[
  {"id":"S1","status":"Available"},
  {"id":"S2","status":"Standby"},
  {"id":"S3","status":"Full"}
]
""";

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
options.Converters.Add(new JsonStringEnumConverter());
var imported = JsonSerializer.Deserialize<List<ShelterUpdate>>(json, options)
    ?? throw new InvalidOperationException("Empty feed");
foreach (var shelter in imported)
    Console.WriteLine($"SHELTER={shelter.Id};STATUS={shelter.Status}");
Console.WriteLine($"IMPORTED={imported.Count}");

public enum ShelterStatus { Available, Full, Closed }
public sealed record ShelterUpdate(string Id, ShelterStatus Status);
