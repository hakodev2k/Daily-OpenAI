using Microsoft.AspNetCore.DataProtection;

var root = Path.Combine(Path.GetTempPath(), "unit-sec-003");
if (Directory.Exists(root)) Directory.Delete(root, true);
Directory.CreateDirectory(root);

var instanceAKeys = new DirectoryInfo(Path.Combine(root, "instance-a"));
var instanceBKeys = new DirectoryInfo(Path.Combine(root, "instance-b"));

// Investigation note: which deployment state must be shared for protected payloads
// to remain readable when traffic moves between equivalent application instances?
var providerA = DataProtectionProvider.Create(instanceAKeys, b => b.SetApplicationName("AdminPortal"));
var providerB = DataProtectionProvider.Create(instanceBKeys, b => b.SetApplicationName("AdminPortal"));

var protectorA = providerA.CreateProtector("auth-cookie");
var protectorB = providerB.CreateProtector("auth-cookie");
var payload = protectorA.Protect("user=42;role=operator");

var selfRead = protectorA.Unprotect(payload);
Console.WriteLine($"A_SELF_READ={(selfRead.Contains("user=42") ? "PASS" : "FAIL")}");

try
{
    _ = protectorB.Unprotect(payload);
    Console.WriteLine("B_CROSS_INSTANCE_READ=PASS");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"B_CROSS_INSTANCE_READ=FAIL ({ex.GetType().Name})");
    return 1;
}
