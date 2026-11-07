using System.Globalization;

static string FormatPrice(decimal value) => value.ToString("N2", CultureInfo.CurrentCulture);

static void EuropeanMarketSetup()
{
    // Test setup for a scenario that needs German formatting.
    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
    var actual = FormatPrice(1234.5m);
    if (actual != "1.234,50") throw new Exception($"EU check failed: {actual}");
}

static void DefaultMarketFormattingCheck()
{
    var actual = FormatPrice(1234.5m);
    if (actual != "1,234.50") throw new Exception($"Default check failed: {actual}");
}

static void SetKnownBaseline() => CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

var mode = args.FirstOrDefault() ?? "run";
SetKnownBaseline();

if (mode == "isolated")
{
    DefaultMarketFormattingCheck();
    Console.WriteLine("isolated: PASS");
    return 0;
}

if (mode == "reproduce")
{
    EuropeanMarketSetup();
    Console.WriteLine($"after EU check culture={CultureInfo.CurrentCulture.Name}");
    try
    {
        DefaultMarketFormattingCheck();
        Console.WriteLine("suite: unexpectedly PASS");
        return 2;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.WriteLine("suite: EXPECTED FAILURE reproduced");
        return 0;
    }
}

if (mode == "verify")
{
    EuropeanMarketSetup();
    DefaultMarketFormattingCheck();
    Console.WriteLine("verification: PASS");
    return 0;
}

Console.WriteLine("Run isolated, reproduce, or verify mode.");
return 0;