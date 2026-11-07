namespace WeighbridgeLab;

public sealed class ReportBuilder
{
    public IntakeReport Build(IReceiptFeed feed)
    {
        ArgumentNullException.ThrowIfNull(feed);

        int count = 0;
        decimal totalWeightKg = 0m;
        foreach (var receipt in feed.ReadCurrent())
        {
            checked { count++; }
            totalWeightKg = checked(totalWeightKg + receipt.WeightKg);
        }

        return new IntakeReport(count, totalWeightKg);
    }
}
