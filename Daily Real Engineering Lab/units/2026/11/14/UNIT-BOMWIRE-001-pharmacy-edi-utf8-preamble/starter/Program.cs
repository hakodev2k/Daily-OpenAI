using System.Text;

var mode = args.Length > 0 ? args[0] : "export";
var file = Path.Combine(AppContext.BaseDirectory, "manifest.edi");
const string payload = "HDR|20261114|PHARMA\nITM|001|Thuốc cảm\nTRL|1\n";

static void Export(string target, string data)
{
    // Legacy exporter configured by a third-party integration team.
    using var writer = new StreamWriter(target, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    writer.Write(data);
}

static bool Inspect(string target)
{
    if (!File.Exists(target)) { Console.Error.WriteLine("MISSING_EXPORT"); return false; }
    var bytes = File.ReadAllBytes(target);
    Console.WriteLine("FIRST_BYTES=" + Convert.ToHexString(bytes.AsSpan(0, Math.Min(12, bytes.Length))));
    var prefixOk = bytes.Length >= 3 && bytes[0] == 0x48 && bytes[1] == 0x44 && bytes[2] == 0x52;
    var expected = "HDR|20261114|PHARMA\nITM|001|Thuốc cảm\nTRL|1\n";
    var textOk = Encoding.UTF8.GetString(bytes) == expected;
    Console.WriteLine("HEADER_AT_OFFSET_ZERO=" + prefixOk);
    Console.WriteLine("EXACT_UTF8_PAYLOAD=" + textOk);
    return prefixOk && textOk;
}

switch (mode)
{
    case "export": Export(file, payload); Console.WriteLine("EXPORTED=" + file); break;
    case "inspect": if (!Inspect(file)) Environment.ExitCode = 2; break;
    case "verify": Export(file, payload); if (!Inspect(file)) Environment.ExitCode = 2; break;
    default: Console.Error.WriteLine("Usage: export|inspect|verify"); Environment.ExitCode = 64; break;
}
