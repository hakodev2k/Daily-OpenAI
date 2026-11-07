using System.Globalization;

static string FormatPrice(decimal value) => value.ToString("N2", CultureInfo.CurrentCulture);

static void WithCulture(string name, Action action)
{
    var previous = CultureInfo.CurrentCulture;
    try
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(name);
        action();
    }
    finally
    {
        CultureInfo.CurrentCulture = previous;
    }
}

static void EuropeanMarketCheck() => WithCulture("de-DE", () =>
{
    var actual = FormatPrice(1234.5m);
    if (actual != "1.234,50") throw new Exception($"EU check failed: {actual}");
});

static void DefaultMarketCheck() => WithCulture("en-US", () =>
{
    var actual = FormatPrice(1234.5m);
    if (actual != "1,234.50") throw new Exception($"Default check failed: {actual}");
});

EuropeanMarketCheck();
DefaultMarketCheck();
Console.WriteLine("solution verification: PASS");