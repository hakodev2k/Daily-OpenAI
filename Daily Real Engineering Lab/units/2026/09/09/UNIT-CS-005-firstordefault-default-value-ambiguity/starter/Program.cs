var overrides = new[]
{
    new MerchantFeeOverride("M-ZERO", 0m),
    new MerchantFeeOverride("M-LOW", 0.75m)
};

foreach (var merchantId in new[] { "M-ZERO", "M-LOW", "M-NONE" })
{
    var matches = overrides.Where(x => x.MerchantId == merchantId).ToArray();
    var fee = ResolveFee(merchantId, overrides);

    Console.WriteLine($"merchant={merchantId} matches={matches.Length} raw={(matches.Length == 0 ? "<none>" : matches[0].FeePercent.ToString("0.##"))} resolved={fee:0.##}");
}

static decimal ResolveFee(string merchantId, IEnumerable<MerchantFeeOverride> overrides)
{
    const decimal defaultFeePercent = 2.5m;

    var configuredFee = overrides
        .Where(x => x.MerchantId == merchantId)
        .Select(x => x.FeePercent)
        .FirstOrDefault();

    if (configuredFee == 0m)
    {
        return defaultFeePercent;
    }

    return configuredFee;
}

internal sealed record MerchantFeeOverride(string MerchantId, decimal FeePercent);
