using System.Text.Json;
using CertificateDropLab;

var mode = args.Length == 0 ? "run" : args[0];
var inbox = Path.Combine(Path.GetTempPath(), "certificate-drop-lab-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(inbox);

try
{
    string Put(string fileName, string certificateId, string deviceId)
    {
        var path = Path.Combine(inbox, fileName);
        File.WriteAllText(path, JsonSerializer.Serialize(new Certificate(certificateId, deviceId)));
        return path;
    }

    var first = Put("cal-101.json", "CAL-101", "SENSOR-11");
    Put("cal-102.json", "CAL-102", "SENSOR-12");
    var third = Put("cal-103.json", "CAL-103", "SENSOR-13");

    var intake = new CertificateIntake(inbox);
    // The fixture supplies a deterministic notification history.
    intake.OnCreated(first);
    intake.OnCreated(third);
    intake.OnCreated(third);
    intake.Reconcile();

    void PrintSnapshot()
    {
        Console.WriteLine("ON_DISK: " +
            string.Join(", ", Directory.GetFiles(inbox, "*.json").Select(Path.GetFileName).OrderBy(x => x, StringComparer.Ordinal)));
        Console.WriteLine("ACCEPTED: " +
            string.Join(", ", intake.Accepted.Select(x => x.CertificateId).OrderBy(x => x, StringComparer.Ordinal)));
    }

    PrintSnapshot();

    if (mode == "reproduce")
    {
        var missing = !intake.Accepted.Any(x => x.CertificateId == "CAL-102");
        var noDuplicate = intake.Accepted.Count == 2 &&
            intake.Accepted.Select(x => x.CertificateId).Distinct(StringComparer.Ordinal).Count() == 2;
        if (missing && noDuplicate)
        {
            Console.WriteLine("REPRO_FAIL: valid certificate absent from accepted register");
            return 2;
        }

        Console.WriteLine("REPRO_MISMATCH: baseline symptom not present");
        return 1;
    }

    if (mode == "verify")
    {
        Put("cal-104.json", "CAL-104", "SENSOR-14");
        intake.Reconcile();
        intake.OnCreated(first);
        intake.Reconcile();

        var expected = new[] { "CAL-101", "CAL-102", "CAL-103", "CAL-104" };
        var actual = intake.Accepted.Select(x => x.CertificateId)
            .OrderBy(x => x, StringComparer.Ordinal).ToArray();
        var correct = actual.SequenceEqual(expected, StringComparer.Ordinal) &&
            intake.Accepted.Count == expected.Length &&
            intake.Accepted.All(x => x.DeviceId.StartsWith("SENSOR-", StringComparison.Ordinal));

        PrintSnapshot();
        Console.WriteLine(correct ? "VERIFY_PASS" : "VERIFY_FAIL: missing or duplicate certificates");
        return correct ? 0 : 1;
    }

    if (mode == "run")
        return 0;

    Console.Error.WriteLine("Unknown mode: " + mode);
    return 64;
}
finally
{
    Directory.Delete(inbox, recursive: true);
}
