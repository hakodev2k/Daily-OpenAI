// Reference Solution — chỉ dùng để so sánh sau khi tự sửa.
// Copy đè starter/Settlement.Core/SettlementCalculator.cs nếu muốn kiểm tra riêng.
namespace Settlement.Core;

public sealed record LedgerEntry(string SellerId, string TransactionId, decimal Amount);
public sealed record SellerStatement(string SellerId, decimal Gross, decimal Fee, decimal Net);

public static class SettlementCalculator
{
    public static IReadOnlyList<SellerStatement> Build(
        IEnumerable<LedgerEntry> entries,
        decimal feeRate)
    {
        ArgumentNullException.ThrowIfNull(entries);
        if (feeRate < 0m || feeRate > 1m)
            throw new ArgumentOutOfRangeException(nameof(feeRate));

        var batch = entries.ToArray();
        if (batch.Any(e => string.IsNullOrWhiteSpace(e.SellerId) ||
                           string.IsNullOrWhiteSpace(e.TransactionId)))
            throw new ArgumentException("SellerId and TransactionId are required.");

        return batch
            .GroupBy(e => e.SellerId, StringComparer.Ordinal)
            .OrderBy(group => group.Key, StringComparer.Ordinal)
            .Select(group =>
            {
                var gross = group.Sum(e => e.Amount);
                var fee = group.Sum(e =>
                    decimal.Round(e.Amount * feeRate, 2, MidpointRounding.AwayFromZero));
                return new SellerStatement(group.Key, gross, fee, gross - fee);
            })
            .ToArray();
    }
}
