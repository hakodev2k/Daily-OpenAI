using System;

var fixtures = new (string Name, byte[] Frame, Reading Expected)[]
{
    ("normal", new byte[] { 0x01, 0x02, 0xD7, 0x00, 0x00, 0x00, 0x00, 0x2A }, new Reading(258, 215, 42)),
    ("negative", new byte[] { 0x00, 0x09, 0xC4, 0xFF, 0x00, 0x00, 0x01, 0x00 }, new Reading(9, -60, 256)),
    ("boundary", new byte[] { 0xFF, 0xFF, 0x00, 0x80, 0xFF, 0xFF, 0xFF, 0xFF }, new Reading(65535, short.MinValue, uint.MaxValue))
};
int failures = 0;
foreach (var fixture in fixtures)
{
    var actual = RiverFrameDecoder.Decode(fixture.Frame);
    bool ok = actual == fixture.Expected;
    Console.WriteLine($"{fixture.Name}: station={actual.StationId}, temperatureDeciC={actual.TemperatureDeciC}, seq={actual.Sequence}; expected={fixture.Expected}; {(ok ? "PASS" : "FAIL")}");
    if (!ok) failures++;
}
try
{
    RiverFrameDecoder.Decode(new byte[7]);
    Console.Error.WriteLine("length-guard: FAIL");
    failures++;
}
catch (ArgumentException)
{
    Console.WriteLine("length-guard: PASS");
}
if (args.Contains("--reproduce", StringComparer.Ordinal))
{
    if (failures == 3)
    {
        Console.WriteLine("REPRODUCED: three temperature assertions fail; station/sequence and length contract still pass.");
        return 0;
    }
    Console.Error.WriteLine($"Reproduction contract changed: {failures} failures; expected 3.");
    return 1;
}
if (failures > 0)
{
    Console.Error.WriteLine($"VERIFY FAILED: {failures} assertions.");
    return 1;
}
Console.WriteLine("VERIFY PASSED: all wire-contract fixtures.");
return 0;
