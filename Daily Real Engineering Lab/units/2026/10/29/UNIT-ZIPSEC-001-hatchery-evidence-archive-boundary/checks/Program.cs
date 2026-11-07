using System.IO.Compression;
using System.Text;
using HatcheryIntake;

if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run --project checks/ArchiveChecks.csproj -- reproduce|verify");
    return 2;
}
if (args[0] == "reproduce")
{
    using var box = new Sandbox();
    using var zip = MakeZip(("docs/reading.txt", "pH=7.2"), ("../outside-review.csv", "status=approved"));
    Console.WriteLine($"intake={box.Destination}");
    Console.WriteLine($"adjacent={Path.Combine(box.Root, "outside-review.csv")}");
    ArchiveIntake.Extract(zip, box.Destination);
    bool inside = File.Exists(Path.Combine(box.Destination, "docs", "reading.txt"));
    bool outside = File.Exists(Path.Combine(box.Root, "outside-review.csv"));
    Console.WriteLine($"inside={inside}; outside={outside}");
    return inside && outside ? 0 : 1;
}

var failures = new List<string>();
Run("valid nested documents", () =>
{
    using var box = new Sandbox();
    using var zip = MakeZip(("docs/reading.txt", "pH=7.2"), ("docs/audit.txt", "signed"));
    ArchiveIntake.Extract(zip, box.Destination);
    Require(File.ReadAllText(Path.Combine(box.Destination, "docs", "reading.txt")) == "pH=7.2", "reading missing");
    Require(File.ReadAllText(Path.Combine(box.Destination, "docs", "audit.txt")) == "signed", "audit missing");
});
Run("parent entry rejected before writing", () =>
{
    using var box = new Sandbox();
    using var zip = MakeZip(("docs/reading.txt", "pH=7.2"), ("../outside-review.csv", "status=approved"));
    Require(Rejected(() => ArchiveIntake.Extract(zip, box.Destination)), "archive must be rejected");
    Require(!File.Exists(Path.Combine(box.Root, "outside-review.csv")), "outside file created");
    Require(!File.Exists(Path.Combine(box.Destination, "docs", "reading.txt")), "partial extraction");
});
Run("sibling-prefix directory rejected", () =>
{
    using var box = new Sandbox();
    using var zip = MakeZip(("../intake-backup/approval.txt", "approved"));
    Require(Rejected(() => ArchiveIntake.Extract(zip, box.Destination)), "sibling path accepted");
    Require(!File.Exists(Path.Combine(box.Root, "intake-backup", "approval.txt")), "sibling file created");
});
Run("backslash entry rejected on every OS", () =>
{
    using var box = new Sandbox();
    using var zip = MakeZip((@"..\escape.txt", "untrusted"));
    Require(Rejected(() => ArchiveIntake.Extract(zip, box.Destination)), "backslash entry accepted");
});
Run("valid deep directory remains supported", () =>
{
    using var box = new Sandbox();
    using var zip = MakeZip(("reports/2026/inspection.csv", "batch,ok"));
    ArchiveIntake.Extract(zip, box.Destination);
    Require(File.ReadAllText(Path.Combine(box.Destination, "reports", "2026", "inspection.csv")) == "batch,ok", "valid file missing");
});
Console.WriteLine($"verification: {5 - failures.Count}/5 checks passed");
foreach (var failure in failures) Console.Error.WriteLine("FAIL: " + failure);
return failures.Count == 0 ? 0 : 1;

void Run(string name, Action action)
{
    try { action(); Console.WriteLine("PASS: " + name); }
    catch (Exception ex) { failures.Add(name + ": " + ex.GetType().Name + " " + ex.Message); }
}
static bool Rejected(Action action)
{
    try { action(); return false; }
    catch (InvalidDataException) { return true; }
}
static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
static MemoryStream MakeZip(params (string Name, string Body)[] entries)
{
    var stream = new MemoryStream();
    using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
    {
        foreach (var (name, body) in entries)
        {
            var entry = zip.CreateEntry(name);
            using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
            writer.Write(body);
        }
    }
    stream.Position = 0;
    return stream;
}
sealed class Sandbox : IDisposable
{
    public string Root { get; } = Path.Combine(Path.GetTempPath(), "hatchery-zip-" + Guid.NewGuid().ToString("N"));
    public string Destination => Path.Combine(Root, "intake");
    public Sandbox() => Directory.CreateDirectory(Root);
    public void Dispose()
    {
        if (Directory.Exists(Root)) Directory.Delete(Root, recursive: true);
    }
}
