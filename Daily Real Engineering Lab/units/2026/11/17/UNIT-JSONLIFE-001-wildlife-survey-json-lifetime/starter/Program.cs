using System.Text.Json;

const string payload = """{"cameraId":"CAM-42","species":"pangolin","count":2}""";

var item = Ingest(payload);
Console.WriteLine("INGEST_ACCEPTED");
var camera = item.GetProperty("cameraId").GetString();
var species = item.GetProperty("species").GetString();
if (camera != "CAM-42" || species != "pangolin")
    throw new InvalidOperationException("Worker received unexpected survey data.");
Console.WriteLine($"WORKER_OK {camera} {species}");

static JsonElement Ingest(string json)
{
    using var document = JsonDocument.Parse(json);
    var root = document.RootElement;
    if (!root.TryGetProperty("cameraId", out _) || !root.TryGetProperty("species", out _))
        throw new FormatException("Missing required survey fields.");
    return root;
}
