using Microsoft.Extensions.Logging;
using System.Globalization;

var mode = args.Length == 0 ? "--run" : args[0];
if (args.Length > 1 || (mode != "--run" && mode != "--reproduce" && mode != "--verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [--run|--reproduce|--verify]");
    return 2;
}
using var sink = new CapturingProvider();
using var factory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Warning).AddProvider(sink));
var logger = factory.CreateLogger("WarrantyClaims");
const string claimId = "WC-4102";
const string partNumber = "AV-29B";
const string region = "APAC";

// Operators must locate affected parts from indexed log attributes during a recall.
logger.LogWarning($"Warranty hold {claimId} for part {partNumber} in {region}");

if (sink.Events.Count != 1)
{
    Console.Error.WriteLine($"Expected one warning event; found {sink.Events.Count}.");
    return 1;
}
var entry = sink.Events[0];
var expectedMessage = $"Warranty hold {claimId} for part {partNumber} in {region}";
var renderedOk = entry.Message == expectedMessage;
var attributesOk = Matches(entry.Properties, "ClaimId", claimId)
    && Matches(entry.Properties, "PartNumber", partNumber)
    && Matches(entry.Properties, "Region", region);
Console.WriteLine("Rendered warning: " + entry.Message);
Console.WriteLine("Indexed attributes: " +
    string.Join(", ", entry.Properties.Where(p => p.Key != "{OriginalFormat}")
        .Select(p => $"{p.Key}={p.Value}")));
if (mode == "--reproduce")
{
    if (renderedOk && !attributesOk)
    {
        Console.WriteLine("REPRODUCED: readable warning exists but indexed attributes are incomplete.");
        return 0;
    }
    Console.Error.WriteLine("REPRODUCTION FAILED: expected symptom was not observed.");
    return 1;
}
if (mode == "--verify")
{
    if (renderedOk && attributesOk)
    {
        Console.WriteLine("VERIFY PASS: readable warning and all indexed attributes agree.");
        return 0;
    }
    Console.Error.WriteLine("VERIFY FAIL: log contract is not satisfied.");
    return 1;
}
Console.WriteLine($"Message contract: {(renderedOk ? "PASS" : "FAIL")}");
Console.WriteLine($"Attribute contract: {(attributesOk ? "PASS" : "FAIL")}");
return 0;

static bool Matches(IReadOnlyDictionary<string, object?> properties, string key, string expected) =>
    properties.TryGetValue(key, out var value) &&
    string.Equals(Convert.ToString(value, CultureInfo.InvariantCulture), expected, StringComparison.Ordinal);

internal sealed record CapturedEntry(string Message, IReadOnlyDictionary<string, object?> Properties);
internal sealed class CapturingProvider : ILoggerProvider
{
    public List<CapturedEntry> Events { get; } = new();
    public ILogger CreateLogger(string categoryName) => new CapturingLogger(Events);
    public void Dispose() { }
    private sealed class CapturingLogger : ILogger
    {
        private readonly List<CapturedEntry> _events;
        public CapturingLogger(List<CapturedEntry> events) => _events = events;
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => EmptyScope.Instance;
        public bool IsEnabled(LogLevel level) => level >= LogLevel.Warning;
        public void Log<TState>(LogLevel level, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(level)) return;
            var properties = state is IEnumerable<KeyValuePair<string, object?>> fields
                ? fields.ToDictionary(p => p.Key, p => p.Value, StringComparer.Ordinal)
                : new Dictionary<string, object?>(StringComparer.Ordinal);
            _events.Add(new CapturedEntry(formatter(state, exception), properties));
        }
    }
    private sealed class EmptyScope : IDisposable
    {
        public static readonly EmptyScope Instance = new();
        public void Dispose() { }
    }
}
