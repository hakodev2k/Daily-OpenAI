namespace WeighbridgeLab;

public sealed class ReportBuilder
{
    public IntakeReport Build(IReceiptFeed feed)
    {
        ArgumentNullException.ThrowIfNull(feed);
        var receipts = feed.ReadCurrent();

        var count = receipts.Count();
        var totalWeightKg = receipts.Sum(x => x.WeightKg);

        return new IntakeReport(count, totalWeightKg);
    }
}
