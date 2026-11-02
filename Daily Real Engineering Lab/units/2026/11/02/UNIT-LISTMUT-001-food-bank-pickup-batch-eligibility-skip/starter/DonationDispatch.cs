namespace FoodBankDispatch;

public sealed record DonationLot(string LotId, DateOnly BestBefore, bool IsQuarantined);

public static class DonationDispatch
{
    // Input is an owned, mutable batch. Keep the eligible lots in their original order.
    public static IReadOnlyList<DonationLot> Prepare(List<DonationLot> lots, DateOnly pickupDate)
    {
        ArgumentNullException.ThrowIfNull(lots);

        for (var i = 0; i < lots.Count; i++)
        {
            var lot = lots[i];
            if (lot.IsQuarantined || lot.BestBefore < pickupDate)
            {
                lots.RemoveAt(i);
            }
        }

        return lots;
    }
}
