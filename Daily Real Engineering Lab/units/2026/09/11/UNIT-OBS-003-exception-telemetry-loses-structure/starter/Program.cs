using Microsoft.Extensions.Logging;

var mode = args.FirstOrDefault() ?? "run";
var logger = new CaptureLogger<PaymentWorker>();
var worker = new PaymentWorker(logger);

await worker.ProcessAsync("ORD-1042");

var entry = logger.LastEntry ?? throw new InvalidOperationException("Expected one log entry.");
Console.WriteLine($"Level={entry.Level}");
Console.WriteLine($"Message={entry.Message}");
Console.WriteLine($"StructuredException={(entry.Exception is null ? "<null>" : entry.Exception.GetType().Name)}");

if (mode.Equals("reproduce", StringComparison.OrdinalIgnoreCase))
{
    if (entry.Level != LogLevel.Error || entry.Exception is not null || !entry.Message.Contains("ORD-1042", StringComparison.Ordinal))
    {
        Console.Error.WriteLine("Starter symptom was not reproduced.");
        return 2;
    }

    Console.WriteLine("REPRODUCED: error message exists, but structured exception is missing.");
    return 0;
}

if (mode.Equals("verify", StringComparison.OrdinalIgnoreCase))
{
    if (entry.Level != LogLevel.Error)
    {
        Console.Error.WriteLine("VERIFY FAILED: expected Error level.");
        return 3;
    }

    if (entry.Exception is not InvalidOperationException)
    {
        Console.Error.WriteLine("VERIFY FAILED: exception was not preserved in the structured exception channel.");
        return 4;
    }

    if (!entry.Message.Contains("ORD-1042", StringComparison.Ordinal))
    {
        Console.Error.WriteLine("VERIFY FAILED: business context OrderId was lost.");
        return 5;
    }

    Console.WriteLine("VERIFY PASSED: exception structure and business context are preserved.");
    return 0;
}

return 0;