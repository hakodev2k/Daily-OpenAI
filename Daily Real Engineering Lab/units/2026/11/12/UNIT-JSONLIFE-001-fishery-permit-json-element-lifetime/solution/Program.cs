using System.Text.Json;
var messages = new[] {
    """{"payload":{"permitId":"FP-102","quotaKg":125}}""",
    """{"payload":{"permitId":"FP-103","quotaKg":240}}"""
};
static JsonElement Parse(string raw) {
    using var doc = JsonDocument.Parse(raw);
    var payload = doc.RootElement.GetProperty("payload");
    Console.WriteLine("Partner payload accepted");
    return payload.Clone();
}
static string Reconcile(JsonElement payload) =>
    $"{payload.GetProperty("permitId").GetString()}:{payload.GetProperty("quotaKg").GetInt32()}";
var mode = args.FirstOrDefault() ?? "run";
try {
    var actual = messages.Select(Parse).Select(Reconcile).ToArray();
    if (!actual.SequenceEqual(new[] { "FP-102:125", "FP-103:240" })) return 4;
    Console.WriteLine("PASS: 2 permits");
    return mode == "reproduce" ? 3 : 0;
} catch (ObjectDisposedException ex) {
    Console.Error.WriteLine($"OBSERVED: {ex.GetType().Name}");
    if (mode == "reproduce") return 0;
    Console.Error.WriteLine(ex);
    return 2;
}
