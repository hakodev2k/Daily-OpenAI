using System.Text.Json;
using RailDepot;

const string Valid = """{"schemaVersion":1,"reservationId":"R-100","partCode":"BRK-22","reservedUnits":7}""";
const string Drift = """{"schemaVersion":1,"reservationId":"R-101","partCode":"BRK-22","reserved_units":7}""";
const string Missing = """{"schemaVersion":1,"reservationId":"R-102","partCode":"BRK-22"}""";
const string Negative = """{"schemaVersion":1,"reservationId":"R-103","partCode":"BRK-22","reservedUnits":-2}""";
const string NewVersion = """{"schemaVersion":2,"reservationId":"R-104","partCode":"BRK-22","reservedUnits":7}""";
const string Unknown = """{"schemaVersion":1,"reservationId":"R-105","partCode":"BRK-22","reservedUnits":7,"reserveNote":"fragile"}""";
const string Zero = """{"schemaVersion":1,"reservationId":"R-106","partCode":"BRK-22","reservedUnits":0}""";
const string EmptyId = """{"schemaVersion":1,"reservationId":" ","partCode":"BRK-22","reservedUnits":3}""";

string mode = args.Length == 1 ? args[0] : "--observe";
if (mode == "--observe")
{
    foreach (var (name, payload) in new[] { ("v1", Valid), ("partner-drift", Drift), ("missing-units", Missing) })
    {
        try
        {
            var value = ReservationReader.Read(payload);
            Console.WriteLine($"{name}: accepted id={value.ReservationId}, units={value.ReservedUnits}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"{name}: rejected {ex.GetType().Name}");
        }
    }
    return 0;
}

if (mode == "--reproduce")
{
    try
    {
        var normal = ReservationReader.Read(Valid);
        var changed = ReservationReader.Read(Drift);
        var missing = ReservationReader.Read(Missing);
        bool observed = normal.ReservedUnits == 7 && changed.ReservedUnits == 0 && missing.ReservedUnits == 0;
        Console.WriteLine($"normal={normal.ReservedUnits}; changed={changed.ReservedUnits}; missing={missing.ReservedUnits}; symptomObserved={observed}");
        return observed ? 0 : 1;
    }
    catch (JsonException)
    {
        Console.WriteLine("Starter symptom not reproduced (input was rejected).");
        return 1;
    }
}

if (mode != "--verify")
{
    Console.Error.WriteLine("Usage: --observe | --reproduce | --verify");
    return 2;
}

var cases = new (string Name, string Json, bool Accept, int ExpectedUnits)[]
{
    ("valid-v1", Valid, true, 7),
    ("explicit-zero", Zero, true, 0),
    ("field-rename", Drift, false, 0),
    ("missing-field", Missing, false, 0),
    ("negative-units", Negative, false, 0),
    ("unsupported-version", NewVersion, false, 0),
    ("unrecognized-field", Unknown, false, 0),
    ("empty-id", EmptyId, false, 0)
};
int failed = 0;
foreach (var test in cases)
{
    bool accepted;
    ReservationPayload? result = null;
    try
    {
        result = ReservationReader.Read(test.Json);
        accepted = true;
    }
    catch (JsonException)
    {
        accepted = false;
    }
    bool pass = accepted == test.Accept &&
        (!accepted || (result is not null && result.ReservedUnits == test.ExpectedUnits &&
                        !string.IsNullOrWhiteSpace(result.ReservationId) &&
                        result.SchemaVersion == 1));
    Console.WriteLine($"{(pass ? "PASS" : "FAIL")} {test.Name}: expected={(test.Accept ? "ACCEPT" : "REJECT")}, actual={(accepted ? "ACCEPT" : "REJECT")}");
    if (!pass) failed++;
}
Console.WriteLine($"verification: {cases.Length - failed}/{cases.Length} passed");
return failed == 0 ? 0 : 1;
