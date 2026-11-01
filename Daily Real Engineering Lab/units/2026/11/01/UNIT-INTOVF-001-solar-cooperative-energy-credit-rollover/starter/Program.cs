internal static class Program
{
    private const int DailyWhPerSite = 65_000;

    private static int Main(string[] args)
    {
        try
        {
            return (args.FirstOrDefault() ?? "demo") switch
            {
                "demo" => Demo(),
                "reproduce" => Reproduce(),
                "verify" => Verify(),
                _ => throw new ArgumentException("Mode: demo, reproduce hoặc verify.")
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 2;
        }
    }

    private static MeterDelta[] BuildReadings(int siteCount) =>
        Enumerable.Range(1, siteCount)
            .Select(i => new MeterDelta($"SITE-{i:D5}", DailyWhPerSite))
            .ToArray();

    private static long Actual(int siteCount) =>
        EnergySettlement.CalculateTotalWh(BuildReadings(siteCount));

    private static long Expected(int siteCount) =>
        (long)siteCount * DailyWhPerSite;

    private static int Demo()
    {
        foreach (var count in new[] { 12_000, 48_000 })
            Console.WriteLine($"Sites={count}; expectedWh={Expected(count)}; actualWh={Actual(count)}");
        return 0;
    }

    private static int Reproduce()
    {
        long small = Actual(12_000);
        long large = Actual(48_000);
        if (small != 780_000_000L)
            throw new InvalidOperationException($"Baseline nhỏ thay đổi: {small}.");

        if (large == 3_120_000_000L)
            throw new InvalidOperationException(
                "Không tái hiện được lỗi; có thể starter đã sửa. Hãy chạy verify.");

        Console.WriteLine($"REPRODUCED: sites=48000 expectedWh=3120000000 actualWh={large}");
        return 0;
    }

    private static void AssertTotal(int siteCount)
    {
        long expected = Expected(siteCount);
        long actual = Actual(siteCount);
        if (actual != expected)
            throw new InvalidOperationException(
                $"FAIL sites={siteCount}: expectedWh={expected}, actualWh={actual}.");
    }

    private static int Verify()
    {
        AssertTotal(0);
        AssertTotal(12_000);
        AssertTotal(33_038);
        AssertTotal(33_039);
        AssertTotal(48_000);

        var readings = BuildReadings(4);
        long first = EnergySettlement.CalculateTotalWh(readings);
        long second = EnergySettlement.CalculateTotalWh(readings);
        if (first != 260_000L || second != first)
            throw new InvalidOperationException("Kết quả không ổn định hoặc thay đổi dữ liệu đầu vào.");

        bool rejected = false;
        try
        {
            _ = EnergySettlement.CalculateTotalWh(
                new[] { new MeterDelta("SITE-NEG", -1) });
        }
        catch (ArgumentOutOfRangeException)
        {
            rejected = true;
        }

        if (!rejected)
            throw new InvalidOperationException("Đã chấp nhận số đo âm.");

        Console.WriteLine("PASS: empty, small, threshold edges, large, repeatability, negative-input rejection.");
        return 0;
    }
}
