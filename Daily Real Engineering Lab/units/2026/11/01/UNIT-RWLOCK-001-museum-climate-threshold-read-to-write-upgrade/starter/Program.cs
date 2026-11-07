using System.Threading;
using System.Threading.Tasks;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            return (args.Length == 0 ? "demo" : args[0]) switch
            {
                "demo" => Demo(),
                "reproduce" => Reproduce(),
                "verify" => Verify(),
                _ => throw new ArgumentException("Mode phải là demo, reproduce hoặc verify.")
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 2;
        }
    }

    private static int Demo()
    {
        using var registry = new ThresholdRegistry();
        Console.WriteLine($"STABLE={registry.GetOrCreate("STABLE")}");
        Console.WriteLine($"NEW={registry.GetOrCreate("NEW")}");
        return 0;
    }

    private static int Reproduce()
    {
        using var registry = new ThresholdRegistry();
        if (registry.GetOrCreate("STABLE") != 8)
            throw new InvalidOperationException("Baseline đã thay đổi.");

        try
        {
            _ = registry.GetOrCreate("NEW");
        }
        catch (LockRecursionException ex)
        {
            Console.WriteLine($"REPRODUCED: {ex.GetType().Name}");
            Console.WriteLine(ex);
            return 0;
        }

        throw new InvalidOperationException(
            "Không tái hiện được lỗi dự kiến. Starter có thể đã được sửa; hãy chạy verify.");
    }

    private static int Verify()
    {
        using var registry = new ThresholdRegistry();
        if (registry.GetOrCreate("STABLE") != 8)
            throw new InvalidOperationException("Giá trị STABLE bị thay đổi.");

        var values = new int[24];
        Parallel.For(0, values.Length, i =>
        {
            values[i] = registry.GetOrCreate("NEW");
        });

        if (values.Any(value => value != 12))
            throw new InvalidOperationException("Một request nhận sai ngưỡng.");
        if (registry.CreatedCount != 1)
            throw new InvalidOperationException(
                $"Một mã chỉ được tạo một lần; observed={registry.CreatedCount}.");

        if (registry.GetOrCreate("NEW") != 12 || registry.CreatedCount != 1)
            throw new InvalidOperationException("Cache hit làm phát sinh thêm bản ghi.");
        if (registry.GetOrCreate("ANOTHER") != 12 || registry.CreatedCount != 2)
            throw new InvalidOperationException("Mã độc lập không được tạo đúng.");

        Console.WriteLine("PASS: cache hit, cache miss, 24 concurrent readers, exactly-once creation.");
        return 0;
    }
}
