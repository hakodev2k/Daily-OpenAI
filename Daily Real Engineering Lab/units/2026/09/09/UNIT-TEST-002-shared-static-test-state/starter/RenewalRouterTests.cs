using Xunit;

namespace SharedStateLab;

public sealed class StripeRenewalTests
{
    [Fact]
    public async Task Routes_new_market_to_stripe()
    {
        RenewalProviderSettings.Provider = "Stripe";
        await ParallelGate.RendezvousAsync("Stripe");

        var router = new RenewalRouter();
        Assert.Equal("Stripe", router.SelectProvider("sub-new-001"));
    }
}

public sealed class LegacyRenewalTests
{
    [Fact]
    public async Task Keeps_existing_market_on_legacy()
    {
        RenewalProviderSettings.Provider = "Legacy";
        await ParallelGate.RendezvousAsync("Legacy");

        var router = new RenewalRouter();
        Assert.Equal("Legacy", router.SelectProvider("sub-old-001"));
    }
}
