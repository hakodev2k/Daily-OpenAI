using System.Security.Cryptography;

var root = Path.Combine(AppContext.BaseDirectory, "lab-work");
var publish = Path.Combine(root, "publish");
var workspace = Path.Combine(root, "workspace");
var package = Path.Combine(root, "package");
Directory.CreateDirectory(publish);
Directory.CreateDirectory(workspace);
Directory.CreateDirectory(package);

File.WriteAllText(Path.Combine(publish, "OrderApi.dll"), "release-binary-v1");
File.WriteAllText(Path.Combine(publish, "appsettings.json"), "{\"release\":1}");
File.WriteAllText(Path.Combine(workspace, "OrderApi.dll"), "release-binary-v1");
File.WriteAllText(Path.Combine(workspace, "appsettings.json"), "{\"release\":1}");
File.WriteAllText(Path.Combine(workspace, "old-plugin.dll"), "stale-file-from-previous-run");

// Investigation note: which directory is the authoritative release artifact?
foreach (var file in Directory.GetFiles(workspace))
    File.Copy(file, Path.Combine(package, Path.GetFileName(file)), true);

static string Manifest(string dir) => string.Join("\n", Directory.GetFiles(dir).OrderBy(Path.GetFileName).Select(f => $"{Path.GetFileName(f)}:{Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(f)))}"));
var expected = Manifest(publish);
var actual = Manifest(package);
Console.WriteLine("PUBLISH\n" + expected);
Console.WriteLine("PACKAGE\n" + actual);

if (args.Contains("reproduce")) {
    if (expected != actual) { Console.WriteLine("REPRODUCED"); return 2; }
    Console.WriteLine("NOT_REPRODUCED"); return 1;
}
if (args.Contains("verify")) {
    if (expected == actual) { Console.WriteLine("VERIFY_PASS"); return 0; }
    Console.WriteLine("VERIFY_FAIL"); return 3;
}
return 0;
