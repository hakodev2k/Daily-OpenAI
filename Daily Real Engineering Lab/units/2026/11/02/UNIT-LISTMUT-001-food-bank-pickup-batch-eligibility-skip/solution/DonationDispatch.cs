namespace FoodBankDispatch;

public sealed record DonationLot(string LotId, DateOnly BestBefore, bool IsQuarantined);

public static class DonationDispatch
{
    public static IReadOnlyList<DonationLot> Prepare(List<DonationLot> lots, DateOnly pickupDate)
    {
        ArgumentNullException.ThrowIfNull(lots);
        lots.RemoveAll(lot => lot.IsQuarantined || lot.BestBefore < pickupDate);
        return lots;
    }
}
