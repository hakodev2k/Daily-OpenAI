using System.Globalization;
using System.Text.Json;

internal sealed record DispatchEnvelope(string Partner, JsonElement Payload);

internal static class CaptureStage
{
    public static DispatchEnvelope Capture(string raw)
    {
        using JsonDocument document = JsonDocument.Parse(raw);
        JsonElement root = document.RootElement;
        string partner = root.GetProperty("partner").GetString()
            ?? throw new FormatException("Partner must be present.");
        JsonElement payload = root.GetProperty("payload");
        return new DispatchEnvelope(partner, payload);
    }
}

internal static class DispatchStage
{
    public static string Render(DispatchEnvelope envelope)
    {
        string eventId = envelope.Payload.GetProperty("eventId").GetString()
            ?? throw new FormatException("Event ID must be present.");
        decimal amount = envelope.Payload.GetProperty("amount").GetDecimal();
        return $"{envelope.Partner}|{eventId}|{amount.ToString(CultureInfo.InvariantCulture)}";
    }
}

internal static class Program
{
    private static readonly (string Json, string Expected)[] Cases =
    {
        ("""{"partner":"north","payload":{"eventId":"EV-101","amount":12.5}}""", "north|EV-101|12.5"),
        ("""{"partner":"south","payload":{"eventId":"EV-205","amount":0}}""", "south|EV-205|0"),
        ("""{"partner":"east","payload":{"eventId":"EV-306","amount":9.25,"details":{"warehouse":"HN"}}}""", "east|EV-306|9.25")
    };

    private static int Main()
    {
        for (int i = 0; i < Cases.Length; i++)
        {
            try
            {
                Console.WriteLine($"CAPTURE item={i + 1}");
                DispatchEnvelope envelope = CaptureStage.Capture(Cases[i].Json);
                Console.WriteLine($"DISPATCH item={i + 1}");
                string actual = DispatchStage.Render(envelope);
                if (!StringComparer.Ordinal.Equals(actual, Cases[i].Expected))
                {
                    Console.Error.WriteLine($"CHECK_FAILED item={i + 1} type=Assertion expected={Cases[i].Expected} actual={actual}");
                    return 3;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"CHECK_FAILED item={i + 1} type={ex.GetType().Name} message={ex.Message}");
                return 2;
            }
        }

        Console.WriteLine("VERIFICATION_PASSED");
        return 0;
    }
}
