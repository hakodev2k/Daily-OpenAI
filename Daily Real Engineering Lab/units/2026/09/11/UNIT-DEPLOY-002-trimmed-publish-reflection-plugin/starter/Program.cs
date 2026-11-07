using System.Text.Json;

namespace TrimLab;

public interface IReportFormatter
{
    string Format(string input);
}

public sealed class CompactReportFormatter : IReportFormatter
{
    public string Format(string input) => $"FORMATTED:{input.ToUpperInvariant()}";
}

public sealed record PluginSettings(string FormatterType);

public static class Program
{
    public static int Main()
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "plugin.json");
        var settings = JsonSerializer.Deserialize<PluginSettings>(File.ReadAllText(configPath))
            ?? throw new InvalidOperationException("Invalid plugin configuration.");

        // Investigation note: the configured component is selected at runtime.
        // Compare what assumptions this path makes in local and published builds.
        var formatterType = Type.GetType(settings.FormatterType, throwOnError: false);
        if (formatterType is null)
        {
            Console.Error.WriteLine("PLUGIN_NOT_FOUND");
            return 2;
        }

        if (Activator.CreateInstance(formatterType) is not IReportFormatter formatter)
        {
            Console.Error.WriteLine("PLUGIN_INVALID");
            return 3;
        }

        Console.WriteLine(formatter.Format("acme"));
        return 0;
    }
}