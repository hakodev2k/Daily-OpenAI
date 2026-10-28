using System.Text.Json;

namespace SlotSwapLab;

public sealed class AppSetting
{
    public string Value { get; set; } = "";
    public bool SlotSetting { get; set; }
}

public sealed class SlotSnapshot
{
    public string Name { get; set; } = "";
    public Dictionary<string, AppSetting> Settings { get; set; } = new(StringComparer.Ordinal);
}

public sealed class DeploymentConfig
{
    public SlotSnapshot Production { get; set; } = new();
    public SlotSnapshot Staging { get; set; } = new();
}

public static class SlotSwapEngine
{
    public static DeploymentConfig Load(string filePath)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<DeploymentConfig>(File.ReadAllText(filePath), options)
            ?? throw new InvalidOperationException("Deployment fixture is empty.");
    }

    // Simplified local model: slot-specific app settings stay with their slot;
    // all other app settings move to the opposite slot.
    public static DeploymentConfig Swap(DeploymentConfig current)
    {
        var prod = current.Production.Settings;
        var stage = current.Staging.Settings;
        if (prod.Count != stage.Count || prod.Keys.Except(stage.Keys, StringComparer.Ordinal).Any())
            throw new InvalidOperationException("Both slots must define identical app setting names.");

        var result = new DeploymentConfig
        {
            Production = new SlotSnapshot { Name = "production" },
            Staging = new SlotSnapshot { Name = "staging" }
        };

        foreach (var key in prod.Keys)
        {
            var p = prod[key];
            var s = stage[key];
            if (p.SlotSetting != s.SlotSetting)
                throw new InvalidOperationException($"Inconsistent slot-specific flag for {key}.");

            result.Production.Settings[key] = new AppSetting
            {
                Value = p.SlotSetting ? p.Value : s.Value,
                SlotSetting = p.SlotSetting
            };
            result.Staging.Settings[key] = new AppSetting
            {
                Value = s.SlotSetting ? s.Value : p.Value,
                SlotSetting = s.SlotSetting
            };
        }

        return result;
    }
}
