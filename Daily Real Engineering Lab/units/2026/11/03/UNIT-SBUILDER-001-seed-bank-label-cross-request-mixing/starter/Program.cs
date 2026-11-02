using System.Globalization;
using System.Text;

return await ContractRunner.RunAsync(args);

internal sealed record Shipment(string LotId, string Facility, string Route);

internal interface ITariffGateway
{
    Task<decimal> GetFeeAsync(string lotId);
}

internal sealed class ImmediateTariffGateway : ITariffGateway
{
    public Task<decimal> GetFeeAsync(string lotId) =>
        Task.FromResult(lotId == "LOT-A" ? 17m : 42m);
}

internal sealed class ControlledTariffGateway : ITariffGateway
{
    private readonly Dictionary<string, TaskCompletionSource<decimal>> _pending =
        new(StringComparer.Ordinal)
        {
            ["LOT-A"] = new(TaskCreationOptions.RunContinuationsAsynchronously),
            ["LOT-B"] = new(TaskCreationOptions.RunContinuationsAsynchronously)
        };

    public Task<decimal> GetFeeAsync(string lotId) => _pending[lotId].Task;

    public void Release(string lotId, decimal fee) => _pending[lotId].SetResult(fee);
}

internal sealed class LabelComposer
{
    private readonly ITariffGateway _tariff;
    private readonly StringBuilder _buffer = new();

    public LabelComposer(ITariffGateway tariff) => _tariff = tariff;

    public async Task<string> BuildAsync(Shipment shipment)
    {
        var buffer = _buffer;
        buffer.Clear();
        buffer.Append("Label|").Append(shipment.LotId).Append('|')
            .Append(shipment.Facility).Append('|').Append(shipment.Route).Append('\n');

        decimal fee = await _tariff.GetFeeAsync(shipment.LotId);

        buffer.Append("Fee|")
            .Append(fee.ToString("0.00", CultureInfo.InvariantCulture))
            .Append('\n');
        return buffer.ToString();
    }
}

internal static class ContractRunner
{
    public static async Task<int> RunAsync(string[] args)
    {
        var a = new Shipment("LOT-A", "North-Vault", "R-01");
        var b = new Shipment("LOT-B", "South-Vault", "R-02");

        var sequentialComposer = new LabelComposer(new ImmediateTariffGateway());
        string seqA = await sequentialComposer.BuildAsync(a);
        string seqB = await sequentialComposer.BuildAsync(b);
        bool sequentialPass = seqA == Expected(a, 17m) && seqB == Expected(b, 42m);

        var controlled = new ControlledTariffGateway();
        var composer = new LabelComposer(controlled);
        Task<string> taskA = composer.BuildAsync(a);
        Task<string> taskB = composer.BuildAsync(b);
        controlled.Release("LOT-B", 42m);
        string resultB = await taskB;
        controlled.Release("LOT-A", 17m);
        string resultA = await taskA;

        bool overlapPass = resultA == Expected(a, 17m) &&
                           resultB == Expected(b, 42m);
        bool crossRequestLeak = resultA.Contains("LOT-B", StringComparison.Ordinal);

        Console.WriteLine("Sequential contract: " + (sequentialPass ? "PASS" : "FAIL"));
        Console.WriteLine("Overlapping contract: " + (overlapPass ? "PASS" : "FAIL"));
        Console.WriteLine("A result: " + resultA.Replace("\n", "\\n", StringComparison.Ordinal));
        Console.WriteLine("B result: " + resultB.Replace("\n", "\\n", StringComparison.Ordinal));

        string mode = args.Length == 0 ? "--demo" : args[0];
        return mode switch
        {
            "--reproduce" => sequentialPass && !overlapPass && crossRequestLeak ? 0 : 1,
            "--verify" => sequentialPass && overlapPass && !crossRequestLeak ? 0 : 1,
            "--demo" => 0,
            _ => 2
        };
    }

    private static string Expected(Shipment shipment, decimal fee) =>
        "Label|" + shipment.LotId + "|" + shipment.Facility + "|" +
        shipment.Route + "\nFee|" +
        fee.ToString("0.00", CultureInfo.InvariantCulture) + "\n";
}
