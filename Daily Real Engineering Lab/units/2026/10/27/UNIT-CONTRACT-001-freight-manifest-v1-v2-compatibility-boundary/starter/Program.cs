using FreightContracts;

var cases = new (string Name, string Json, bool Accept, decimal? Weight, string? Hazard)[]
{
    ("legacy v1 valid", """{"manifestId":"M-101","partnerId":"P-A","weightKg":12.5}""",
        true, 12.5m, "NOT_APPLICABLE"),
    ("v2 standard", """{"schemaVersion":2,"manifestId":"M-102","partnerId":"P-B","grossWeightKg":14.25,"hazardClass":"NONE"}""",
        true, 14.25m, "NONE"),
    ("v2 hazardous", """{"schemaVersion":2,"manifestId":"M-103","partnerId":"P-B","grossWeightKg":8.5,"hazardClass":"FLAMMABLE"}""",
        true, 8.5m, "FLAMMABLE"),
    ("v2 missing hazard declaration", """{"schemaVersion":2,"manifestId":"M-104","partnerId":"P-B","grossWeightKg":11}""",
        false, null, null),
    ("unknown schema version", """{"schemaVersion":3,"manifestId":"M-105","partnerId":"P-B","grossWeightKg":10,"hazardClass":"NONE"}""",
        false, null, null),
    ("mixed v1 and v2 weight fields", """{"schemaVersion":2,"manifestId":"M-106","partnerId":"P-B","weightKg":1,"grossWeightKg":10,"hazardClass":"NONE"}""",
        false, null, null),
    ("v1 invalid weight", """{"manifestId":"M-107","partnerId":"P-A","weightKg":0}""",
        false, null, null),
    ("v2 missing partner identity", """{"schemaVersion":2,"manifestId":"M-108","grossWeightKg":10,"hazardClass":"NONE"}""",
        false, null, null)
};
var failures = 0;
foreach (var test in cases)
{
    try
    {
        var actual = ManifestIngress.Parse(test.Json);
        if (!test.Accept)
        {
            Console.WriteLine($"FAIL {test.Name}: invalid payload accepted");
            failures++;
        }
        else if (test.Weight is not decimal expectedWeight ||
                 actual.GrossWeightKg != expectedWeight ||
                 actual.HazardClass != test.Hazard)
        {
            Console.WriteLine($"FAIL {test.Name}: wrong canonical data: {actual}");
            failures++;
        }
        else Console.WriteLine($"PASS {test.Name}");
    }
    catch (Exception ex) when (ex is System.Text.Json.JsonException or InvalidDataException)
    {
        if (test.Accept)
        {
            Console.WriteLine($"FAIL {test.Name}: valid payload rejected: {ex.Message}");
            failures++;
        }
        else Console.WriteLine($"PASS {test.Name}: invalid payload rejected");
    }
}
Console.WriteLine($"Contract cases: {cases.Length}, failures: {failures}");
return failures == 0 ? 0 : 1;
