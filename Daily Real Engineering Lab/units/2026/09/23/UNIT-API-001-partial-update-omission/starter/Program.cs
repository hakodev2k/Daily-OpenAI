var settings = new NotificationSettings("Merchant A", true);
Console.WriteLine($"Before: {settings}");
var request = new UpdateNotificationSettingsRequest { DisplayName = "Merchant Alpha" };
ApplyUpdate(settings, request);
Console.WriteLine($"After : {settings}");

static void ApplyUpdate(NotificationSettings target, UpdateNotificationSettingsRequest request)
{
    if (request.DisplayName is not null) target.DisplayName = request.DisplayName;
    target.EmailEnabled = request.EmailEnabled;
}

sealed class NotificationSettings(string displayName, bool emailEnabled)
{
    public string DisplayName { get; set; } = displayName;
    public bool EmailEnabled { get; set; } = emailEnabled;
    public override string ToString() => $"DisplayName={DisplayName}, EmailEnabled={EmailEnabled}";
}

sealed class UpdateNotificationSettingsRequest
{
    public string? DisplayName { get; init; }
    public bool EmailEnabled { get; init; }
}
