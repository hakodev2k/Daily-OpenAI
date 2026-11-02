namespace ReusableCrateRefunds;

public static class DepositRefund
{
    // signedQuantity: số thùng hoàn trả; số âm là đảo bút toán.
    // unitDeposit: tiền cọc mỗi thùng, không âm.
    // Kết quả: số tiền ghi sổ, chính xác 2 chữ số thập phân.
    public static decimal Calculate(int signedQuantity, decimal unitDeposit)
    {
        if (unitDeposit < 0m)
            throw new ArgumentOutOfRangeException(nameof(unitDeposit));

        decimal total = signedQuantity * unitDeposit;
        return Math.Round(total, 2);
    }
}
