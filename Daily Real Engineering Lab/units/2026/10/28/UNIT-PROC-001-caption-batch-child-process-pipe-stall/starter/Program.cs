namespace CaptionPipelineLab;

internal static class Program
{
    private const int DiagnosticSize = 1_048_576;

    private static async Task<int> Main(string[] args)
    {
        try
        {
            if (args.Length != 1) return Usage();
            return args[0] switch
            {
                "--worker" => EmitWorkerOutput(),
                "--run" => await ObserveAsync(),
                "--reproduce" => await ReproduceAsync(),
                "--verify" => await VerifyAsync(),
                _ => Usage()
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
            return 1;
        }
    }

    private static int Usage()
    {
        Console.Error.WriteLine("Use --run, --worker, --reproduce or --verify.");
        return 2;
    }

    // Deterministic local stand-in for a caption converter.
    private static int EmitWorkerOutput()
    {
        Console.Error.Write(new string('D', DiagnosticSize));
        Console.Error.Flush();
        Console.Out.WriteLine("CAPTION_READY:314");
        Console.Out.Flush();
        return 0;
    }

    private static async Task<int> ObserveAsync()
    {
        try
        {
            var result = await ConversionRunner.RunAsync();
            Console.WriteLine($"EXIT_CODE={result.ExitCode}");
            Console.WriteLine($"CAPTION={result.StandardOutput.Trim()}");
            Console.WriteLine($"DIAGNOSTIC_LENGTH={result.StandardError.Length}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"TIMEOUT_OBSERVED={ex.Message}");
        }
        return 0;
    }

    private static async Task<int> ReproduceAsync()
    {
        try
        {
            var result = await ConversionRunner.RunAsync();
            Console.WriteLine($"UNEXPECTED_COMPLETION={result.ExitCode}");
            return 1;
        }
        catch (TimeoutException)
        {
            Console.WriteLine("CHILD_EXIT_NOT_OBSERVED_BEFORE_DEADLINE=True");
            Console.WriteLine("REPRODUCED: caption conversion timed out without a completed result.");
            return 0;
        }
    }

    private static async Task<int> VerifyAsync()
    {
        var result = await ConversionRunner.RunAsync();
        var passed = 0;
        void Check(bool condition, string label)
        {
            if (!condition) throw new InvalidOperationException(label);
            passed++;
            Console.WriteLine("PASS: " + label);
        }

        Check(result.ExitCode == 0, "child exits successfully");
        Check(result.StandardOutput.Trim() == "CAPTION_READY:314", "caption output intact");
        Check(result.StandardError.Length == DiagnosticSize, "complete diagnostic output");
        Check(result.StandardError.All(c => c == 'D'), "diagnostic bytes preserved");
        if (passed != 4) return 1;
        Console.WriteLine("CHECKS_PASSED=4");
        Console.WriteLine("VERIFIED: learner runner preserves output and completes.");
        return 0;
    }
}
