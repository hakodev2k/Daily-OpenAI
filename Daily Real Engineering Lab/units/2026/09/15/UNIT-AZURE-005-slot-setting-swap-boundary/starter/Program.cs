var production = new DeploymentSlot(
    "production",
    "v1",
    new Dictionary<string, SlotConfiguration>(StringComparer.OrdinalIgnoreCase)
    {
        ["EnvironmentName"] = new("Production", StickyToSlot: true),
        ["StorageContainer"] = new("prod-images", StickyToSlot: false)
    });

var staging = new DeploymentSlot(
    "staging",
    "v2",
    new Dictionary<string, SlotConfiguration>(StringComparer.OrdinalIgnoreCase)
    {
        ["EnvironmentName"] = new("Staging", StickyToSlot: true),
        ["StorageContainer"] = new("staging-images", StickyToSlot: false)
    });

SlotSwapSimulator.Swap(staging, production);

Console.WriteLine($"PRODUCTION_VERSION={production.ApplicationVersion}");
Console.WriteLine($"PRODUCTION_ENVIRONMENT={production.Settings["EnvironmentName"].Value}");
Console.WriteLine($"PRODUCTION_STORAGE={production.Settings["StorageContainer"].Value}");

var versionPromoted = production.ApplicationVersion == "v2";
var environmentCorrect = production.Settings["EnvironmentName"].Value == "Production";
var storageCorrect = production.Settings["StorageContainer"].Value == "prod-images";
var mode = args.FirstOrDefault() ?? "--run";

if (mode.Equals("--reproduce", StringComparison.OrdinalIgnoreCase))
{
    var symptomExists = versionPromoted && environmentCorrect && !storageCorrect;
    Console.WriteLine(symptomExists ? "REPRODUCED=YES" : "REPRODUCED=NO");
    return symptomExists ? 0 : 1;
}

if (mode.Equals("--verify", StringComparison.OrdinalIgnoreCase))
{
    var fixedBehavior = versionPromoted && environmentCorrect && storageCorrect;
    Console.WriteLine(fixedBehavior ? "VERIFICATION=PASS" : "VERIFICATION=FAIL");
    return fixedBehavior ? 0 : 1;
}

return 0;

internal sealed record SlotConfiguration(string Value, bool StickyToSlot);

internal sealed class DeploymentSlot
{
    public DeploymentSlot(string name, string applicationVersion, Dictionary<string, SlotConfiguration> settings)
    {
        Name = name;
        ApplicationVersion = applicationVersion;
        Settings = settings;
    }

    public string Name { get; }
    public string ApplicationVersion { get; set; }
    public Dictionary<string, SlotConfiguration> Settings { get; }
}

internal static class SlotSwapSimulator
{
    public static void Swap(DeploymentSlot source, DeploymentSlot target)
    {
        (source.ApplicationVersion, target.ApplicationVersion) =
            (target.ApplicationVersion, source.ApplicationVersion);

        foreach (var key in source.Settings.Keys.Union(target.Settings.Keys).ToArray())
        {
            var sourceSetting = source.Settings[key];
            var targetSetting = target.Settings[key];

            if (sourceSetting.StickyToSlot || targetSetting.StickyToSlot)
            {
                continue;
            }

            source.Settings[key] = targetSetting;
            target.Settings[key] = sourceSetting;
        }
    }
}
