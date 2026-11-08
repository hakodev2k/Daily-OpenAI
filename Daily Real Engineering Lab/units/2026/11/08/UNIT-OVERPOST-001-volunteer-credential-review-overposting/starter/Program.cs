using System.Text.Json;
var stored = new VolunteerProfile { Id = 42, DisplayName = "Lan", Phone = "0900000000", ReviewStatus = "Pending" };
var normal = """{"displayName":"Lan Nguyen","phone":"0911111111"}""";
var attack = """{"displayName":"Lan Nguyen","phone":"0911111111","reviewStatus":"Approved"}""";
var service = new ProfileService();
var a = service.Update(stored, normal);
var b = service.Update(stored, attack);
Console.WriteLine($"ordinary={a.DisplayName}/{a.ReviewStatus}");
Console.WriteLine($"adversarial={b.DisplayName}/{b.ReviewStatus}");
bool pass = a.DisplayName == "Lan Nguyen" && a.Phone == "0911111111" && a.ReviewStatus == "Pending" && b.ReviewStatus == "Pending" && b.Phone == "0911111111";
Console.WriteLine(pass ? "VERIFY PASS" : "VERIFY FAIL");
return pass ? 0 : 1;
public sealed class VolunteerProfile
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string ReviewStatus { get; set; } = "Pending";
}
public sealed class ProfileService
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };
    public VolunteerProfile Update(VolunteerProfile current, string json)
    {
        var incoming = JsonSerializer.Deserialize<VolunteerProfile>(json, Options) ?? throw new ArgumentException("Invalid JSON");
        incoming.Id = current.Id;
        if (string.IsNullOrWhiteSpace(incoming.ReviewStatus)) incoming.ReviewStatus = current.ReviewStatus;
        return incoming;
    }
}
