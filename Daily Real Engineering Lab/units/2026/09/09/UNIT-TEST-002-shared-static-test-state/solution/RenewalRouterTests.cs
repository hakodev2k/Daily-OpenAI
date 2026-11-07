using Xunit;

namespace SharedStateLab;

public sealed class StripeRenewalTests
{
    [Fact]
    public async Task Routes_new_market_to_stripe()
    {
        var router = new RenewalRouter(new RenewalOptions("Stripe"));
        await ParallelGate.RendezvousAsync("Stripe");

        Assert.Equal("Stripe", router.SelectProvider("sub-new-001"));
    }
}

public sealed class LegacyRenewalTests
{
    [Fact]
    public async Task Keeps_existing_market_on_legacy()
    {
        var router = new RenewalRouter(new RenewalOptions("Legacy"));
        await ParallelGate.RendezvousAsync("Legacy");

        Assert.Equal("Legacy", router.SelectProvider("sub-old-001"));
    }
}
