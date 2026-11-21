using System.Text.Json;

public static class ContractChecks
{
    public static int Run(Func<string, Reading> parse)
    {
        int failed = 0;
        void Check(string name, Action action)
        {
            try { action(); Console.WriteLine("PASS " + name); }
            catch (Exception e) { failed++; Console.WriteLine("FAIL " + name + " " + e.Message); }
        }
        Check("canonical-reading", () =>
        {
            var r = parse(Samples.All[0]);
            if (r.DeviceId != "MED-17" || r.TemperatureC != 11.5m) throw new Exception("canonical reading lost");
        });
        Check("firmware-case-variant", () =>
        {
            var r = parse(Samples.All[1]);
            if (r.DeviceId != "MED-17" || r.TemperatureC != 3m) throw new Exception("compatible firmware field rejected");
        });
        Check("ambiguous-temperature-rejected", () =>
        {
            try { parse(Samples.All[2]); }
            catch (JsonException) { return; }
            throw new Exception("conflicting reading was accepted");
        });
        Check("duplicate-identical-spelling-rejected", () =>
        {
            const string payload = """{"DeviceId":"MED-17","TemperatureC":11.5,"TemperatureC":3.0}""";
            try { parse(payload); }
            catch (JsonException) { return; }
            throw new Exception("duplicate key accepted");
        });
        Check("missing-temperature-rejected", () =>
        {
            try { parse("""{"DeviceId":"MED-17"}"""); }
            catch (JsonException) { return; }
            throw new Exception("missing measurement accepted");
        });
        Check("unknown-extra-field-compatible", () =>
        {
            var r = parse("""{"DeviceId":"MED-17","TemperatureC":11.5,"Firmware":"v4"}""");
            if (r.TemperatureC != 11.5m) throw new Exception("unexpected reading");
        });
        Console.WriteLine($"CONTRACT_RESULT failed={failed}");
        return failed == 0 ? 0 : 1;
    }
}
