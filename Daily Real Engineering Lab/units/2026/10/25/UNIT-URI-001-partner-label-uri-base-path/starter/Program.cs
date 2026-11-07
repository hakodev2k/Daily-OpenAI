using System.Net;

var mode = args.Length == 0 ? "--demo" : args[0];
if (mode is not ("--demo" or "--reproduce" or "--verify"))
{
    Console.Error.WriteLine("Usage: --demo | --reproduce | --verify");
    return 2;
}
var prefixes = mode == "--verify"
    ? new[] { "/carrier/labels/v2", "/regional/eu/carrier/labels/v2" }
    : new[] { "/carrier/labels/v2" };
var allVerified = true;
foreach (var prefix in prefixes)
{
    using var handler = new FakePartnerHandler(prefix);
    using var http = new HttpClient(handler)
    {
        BaseAddress = new Uri("https://partner.example.test" + prefix + "/")
    };
    var gateway = new LabelGateway(http);
    var submit = await gateway.SubmitAsync("SHIP-42");
    var status = await gateway.GetStatusAsync("SHIP-42");
    Console.WriteLine("Configured base: " + http.BaseAddress);
    Console.WriteLine("Submit: " + (int)submit + "; Status: " + (int)status);
    foreach (var call in handler.Calls)
        Console.WriteLine("Observed: " + call.Method + " " + call.Path);

    var expectedSubmitPath = prefix + "/labels/submit";
    var expectedStatusPath = prefix + "/labels/status/SHIP-42";
    var verified = submit == HttpStatusCode.Accepted
        && status == HttpStatusCode.OK
        && handler.Calls.Count == 2
        && handler.Calls[0].Method == "POST"
        && handler.Calls[0].Path == expectedSubmitPath
        && handler.Calls[1].Method == "GET"
        && handler.Calls[1].Path == expectedStatusPath;
    if (mode == "--reproduce")
    {
        var reproduced = submit == HttpStatusCode.NotFound
            && status == HttpStatusCode.OK
            && handler.Calls.Count == 2
            && handler.Calls[0].Path == "/labels/submit"
            && handler.Calls[1].Path == expectedStatusPath;
        Console.WriteLine(reproduced ? "REPRODUCED: outbound routing mismatch" : "NOT REPRODUCED");
        return reproduced ? 0 : 1;
    }
    if (mode == "--verify")
        Console.WriteLine(verified ? "PASS: " + prefix : "FAIL: " + prefix);
    allVerified &= verified;
}
if (mode == "--verify")
    Console.WriteLine(allVerified ? "PASS: both API prefixes" : "FAIL: outbound route contract");
return mode == "--verify" && !allVerified ? 1 : 0;
