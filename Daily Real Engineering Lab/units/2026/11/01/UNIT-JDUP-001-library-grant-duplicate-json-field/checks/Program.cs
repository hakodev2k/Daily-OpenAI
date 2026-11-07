using System;

var cases = new (string Name, string Payload, bool Accept, int? Posted)[]
{
    ("valid-under-limit", """{"requestId":"LIB-101","grantAmount":4200}""", true, 4200),
    ("valid-at-limit", """{"requestId":"LIB-102","grantAmount":5000}""", true, 5000),
    ("above-limit", """{"requestId":"LIB-103","grantAmount":5001}""", false, null),
    ("zero", """{"requestId":"LIB-104","grantAmount":0}""", false, null),
    ("duplicate-amount", """{"requestId":"LIB-105","grantAmount":4200,"grantAmount":42000}""", false, null),
    ("duplicate-id", """{"requestId":"LIB-106","requestId":"LIB-999","grantAmount":100}""", false, null),
    ("unknown-field", """{"requestId":"LIB-107","grantAmount":100,"extra":true}""", false, null),
    ("wrong-case", """{"requestId":"LIB-108","GrantAmount":100}""", false, null),
    ("missing-amount", """{"requestId":"LIB-109"}""", false, null),
    ("fractional-amount", """{"requestId":"LIB-110","grantAmount":42.5}""", false, null),
    ("empty-id", """{"requestId":" ","grantAmount":100}""", false, null),
    ("not-object", "[]", false, null),
    ("malformed", """{"requestId":"LIB-111","grantAmount":100""", false, null)
};
var failed = 0;
foreach (var test in cases)
{
    try
    {
        var result = GrantWorkflow.Handle(test.Payload);
        var ok = result.Accepted == test.Accept && result.PersistedAmount == test.Posted;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {test.Name}: accepted={result.Accepted}, posted={result.PersistedAmount}");
        if (!ok) failed++;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"FAIL {test.Name}: unexpected {ex.GetType().Name}: {ex.Message}");
        failed++;
    }
}
Console.WriteLine($"Contract cases: {cases.Length}, failures: {failed}");
return failed == 0 ? 0 : 1;
