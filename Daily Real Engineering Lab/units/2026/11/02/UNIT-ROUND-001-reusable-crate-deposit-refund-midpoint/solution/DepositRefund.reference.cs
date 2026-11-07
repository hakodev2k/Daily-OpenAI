namespace ReusableCrateRefunds;

public static class DepositRefund
{
    public static decimal Calculate(int signedQuantity, decimal unitDeposit)
    {
        if (unitDeposit < 0m)
            throw new ArgumentOutOfRangeException(nameof(unitDeposit));

        decimal total = signedQuantity * unitDeposit;
        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }
}
