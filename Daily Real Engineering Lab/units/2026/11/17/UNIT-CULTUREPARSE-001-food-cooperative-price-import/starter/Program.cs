using System.Globalization;

internal static class Program
{
    private static readonly string[] PartnerPrices = ["1,234.50", "9,876.25"];

    private static decimal ParsePartnerPrice(string raw)
    {
        // Partner contract: decimal point '.', thousands separator ','.
        // Investigate whether the current implementation honors that contract in every worker.
        return decimal.Parse(raw);
    }

    private static decimal ImportTotal() => PartnerPrices.Sum(ParsePartnerPrice);

    private static int Main(string[] args)
    {
        if (args.Contains("--demo"))
        {
            foreach (var locale in new[] { "en-US", "de-DE", "fr-FR" })
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(locale);
                try { Console.WriteLine($"{locale}: {ImportTotal().ToString(CultureInfo.InvariantCulture)}"); }
                catch (FormatException ex) { Console.WriteLine($"{locale}: {ex.GetType().Name}"); }
            }
            return 0;
        }

        if (args.Contains("--verify"))
        {
            const decimal expected = 11110.75m;
            foreach (var locale in new[] { "en-US", "de-DE", "fr-FR", "vi-VN" })
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(locale);
                decimal actual;
                try { actual = ImportTotal(); }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"FAIL {locale}: {ex.GetType().Name}");
                    return 1;
                }
                if (actual != expected)
                {
                    Console.Error.WriteLine($"FAIL {locale}: expected {expected}, got {actual}");
                    return 1;
                }
            }

            foreach (var invalid in new[] { "1.234,50", "12x.00", "" })
            {
                try { _ = ParsePartnerPrice(invalid); }
                catch (FormatException) { continue; }
                Console.Error.WriteLine($"FAIL: accepted invalid partner price '{invalid}'");
                return 1;
            }

            Console.WriteLine("PASS: locale-independent totals and invalid-input contract");
            return 0;
        }

        Console.WriteLine("Use --demo or --verify");
        return 2;
    }
}
