var settings = new NotificationSettings("Merchant A", true);
Console.WriteLine($"Before: {settings}");
ApplyUpdate(settings, new UpdateNotificationSettingsRequest { DisplayName = "Merchant Alpha" });
Console.WriteLine($"After omitted: {settings}");
ApplyUpdate(settings, new UpdateNotificationSettingsRequest { EmailEnabled = false });
Console.WriteLine($"After explicit false: {settings}");

static void ApplyUpdate(NotificationSettings target, UpdateNotificationSettingsRequest request)
{
    if (request.DisplayName is not null) target.DisplayName = request.DisplayName;
    if (request.EmailEnabled is bool emailEnabled) target.EmailEnabled = emailEnabled;
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
    public bool? EmailEnabled { get; init; }
}
