using System.Text.Json;

var input = new[]
{
    """{"id":"F-101","alert":{"zone":"Delta","severity":"high"}}""",
    """{"id":"F-102","alert":{"zone":"Harbor","severity":"critical"}}"""
};

var intake = new FloodAlertIntake();
var pending = input.Select(intake.Accept).ToArray();
var controlValidated = input.Count(FloodAlertIntake.IsValidControl);
var delivered = new List<string>();
var failures = new List<string>();

Console.WriteLine($"intake.accepted={pending.Length} control.validated={controlValidated}");
foreach (var item in pending)
{
    try
    {
        var zone = item.Alert.GetProperty("zone").GetString()
            ?? throw new InvalidDataException("zone missing");
        var severity = item.Alert.GetProperty("severity").GetString()
            ?? throw new InvalidDataException("severity missing");
        delivered.Add($"{item.Id}|{zone}|{severity}");
        Console.WriteLine($"dispatch.delivered id={item.Id} zone={zone} severity={severity}");
    }
    catch (ObjectDisposedException ex)
    {
        failures.Add(ex.GetType().Name);
        Console.WriteLine($"dispatch.failed id={item.Id} type={ex.GetType().Name}");
    }
}
Console.WriteLine($"summary accepted={pending.Length} delivered={delivered.Count} failed={failures.Count} controlValidated={controlValidated}");

var mode = args.FirstOrDefault() ?? "--observe";
if (mode == "--assert-broken")
{
    return pending.Length == 2 && controlValidated == 2 && delivered.Count == 0
        && failures.Count == 2 && failures.All(x => x == nameof(ObjectDisposedException)) ? 0 : 1;
}
if (mode == "--assert-fixed")
{
    return pending.Length == 2 && controlValidated == 2 && failures.Count == 0
        && delivered.SequenceEqual(new[] { "F-101|Delta|high", "F-102|Harbor|critical" }) ? 0 : 1;
}
return mode == "--observe" ? 0 : 2;

internal sealed record PendingAlert(string Id, JsonElement Alert);

internal sealed class FloodAlertIntake
{
    public PendingAlert Accept(string json)
    {
        using var document = JsonDocument.Parse(json);
        var id = document.RootElement.GetProperty("id").GetString()
            ?? throw new InvalidDataException("id missing");
        var alert = document.RootElement.GetProperty("alert");
        return new PendingAlert(id, alert.Clone());
    }

    public static bool IsValidControl(string json)
    {
        using var document = JsonDocument.Parse(json);
        var alert = document.RootElement.GetProperty("alert");
        return !string.IsNullOrWhiteSpace(alert.GetProperty("zone").GetString())
            && !string.IsNullOrWhiteSpace(alert.GetProperty("severity").GetString());
    }
}
