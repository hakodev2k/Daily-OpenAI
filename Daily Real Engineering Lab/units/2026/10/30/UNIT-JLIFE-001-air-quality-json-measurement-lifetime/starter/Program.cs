using System.Text.Json;
using AirQualityLab;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length != 1 || args[0] != "check")
        {
            Console.Error.WriteLine("Usage: dotnet run --project starter -- check");
            return 64;
        }

        var samples = new (string Json, string Station, decimal Pm25, string Unit)[]
        {
            ("""{"measurement":{"stationId":"AQ-HN-017","metrics":{"pm25":12.5,"unit":"ug/m3"}}}""", "AQ-HN-017", 12.5m, "ug/m3"),
            ("""{"measurement":{"stationId":"AQ-DN-042","metrics":{"pm25":38.75,"unit":"ug/m3"}}}""", "AQ-DN-042", 38.75m, "ug/m3")
        };

        int passed = 0;
        foreach (var sample in samples)
        {
            try
            {
                PublishedReading actual = MeasurementPublisher.Publish(sample.Json);
                if (actual.StationId == sample.Station &&
                    actual.Pm25 == sample.Pm25 &&
                    actual.Unit == sample.Unit)
                {
                    passed++;
                    Console.WriteLine($"CASE={sample.Station} RESULT=PASS");
                }
                else
                {
                    Console.WriteLine($"CASE={sample.Station} RESULT=FAIL REASON=IncorrectValues");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CASE={sample.Station} RESULT=FAIL EXCEPTION={ex.GetType().Name}");
            }
        }

        bool invalidJsonRejected = false;
        try
        {
            _ = MeasurementPublisher.Publish("{not-json}");
        }
        catch (JsonException)
        {
            invalidJsonRejected = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CASE=INVALID_JSON RESULT=FAIL EXCEPTION={ex.GetType().Name}");
        }

        Console.WriteLine($"CASE=INVALID_JSON RESULT={(invalidJsonRejected ? "PASS" : "FAIL")}");
        bool success = passed == samples.Length && invalidJsonRejected;
        Console.WriteLine($"PUBLISHED={passed}/{samples.Length}");
        Console.WriteLine($"LAB_RESULT={(success ? "PASS" : "FAIL")}");
        return success ? 0 : 2;
    }
}
