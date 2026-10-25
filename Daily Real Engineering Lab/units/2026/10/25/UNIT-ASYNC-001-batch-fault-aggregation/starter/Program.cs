namespace BatchFaultLab;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        string mode = args.Length == 1 ? args[0] : "--demo";
        return mode switch
        {
            "--demo" => await DemoAsync(),
            "--reproduce" => await ReproduceAsync(),
            "--verify" => await VerifyAsync(),
            _ => Usage()
        };
    }

    private static int Usage()
    {
        Console.Error.WriteLine("Usage: --demo | --reproduce | --verify");
        return 2;
    }

    private static IReadOnlyList<FeedJob> MixedJobs() => new FeedJob[]
    {
        new("North", _ => Task.FromResult(42)),
        new("West", _ => Task.FromException<int>(new TimeoutException("upstream timed out"))),
        new("East", _ => Task.FromException<int>(new InvalidOperationException("unexpected feed schema")))
    };

    private static async Task<int> DemoAsync()
    {
        BatchReport report = await BatchRunner.RunAsync(MixedJobs());
        Console.WriteLine("Report: succeeded=" + report.Succeeded.Count +
                          ", failed=" + report.Failed.Count);
        foreach (FeedSuccess item in report.Succeeded)
            Console.WriteLine("SUCCESS " + item.Partner + " rows=" + item.Rows);
        foreach (FeedFailure item in report.Failed)
            Console.WriteLine("FAIL " + item.Partner + " " + item.ErrorType + ": " + item.Message);
        return 0;
    }

    private static async Task<int> ReproduceAsync()
    {
        BatchReport report = await BatchRunner.RunAsync(MixedJobs());
        bool observed = report.Succeeded.Count == 0 &&
                        report.Failed.Count == 1 &&
                        report.Failed[0].Partner == "batch";
        if (!observed)
        {
            Console.Error.WriteLine("NOT REPRODUCED: original report symptom is absent.");
            return 1;
        }

        Console.WriteLine("REPRODUCED: one generic failure; North success and a second fault are absent.");
        return 0;
    }

    private static async Task<int> VerifyAsync()
    {
        int passed = 0;
        int total = 0;

        async Task Check(string label, Func<Task<bool>> check)
        {
            total++;
            try
            {
                if (await check())
                {
                    passed++;
                    Console.WriteLine("PASS: " + label);
                }
                else
                {
                    Console.Error.WriteLine("FAIL: " + label);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("FAIL: " + label + " threw " +
                                        ex.GetType().Name + ": " + ex.Message);
            }
        }

        await Check("all successful feeds retained", async () =>
        {
            var jobs = new FeedJob[]
            {
                new("North", _ => Task.FromResult(42)),
                new("South", _ => Task.FromResult(7))
            };
            BatchReport result = await BatchRunner.RunAsync(jobs);
            return result.Failed.Count == 0 &&
                   result.Succeeded.Count == 2 &&
                   result.Succeeded.Any(x => x.Partner == "North" && x.Rows == 42) &&
                   result.Succeeded.Any(x => x.Partner == "South" && x.Rows == 7);
        });

        await Check("two distinct failures plus successful source", async () =>
        {
            BatchReport result = await BatchRunner.RunAsync(MixedJobs());
            return result.Succeeded.Count == 1 &&
                   result.Succeeded.Single().Partner == "North" &&
                   result.Succeeded.Single().Rows == 42 &&
                   result.Failed.Count == 2 &&
                   result.Failed.Any(x => x.Partner == "West" && x.ErrorType == "TimeoutException") &&
                   result.Failed.Any(x => x.Partner == "East" && x.ErrorType == "InvalidOperationException");
        });

        await Check("single failed source does not erase successes", async () =>
        {
            var jobs = new FeedJob[]
            {
                new("South", _ => Task.FromResult(9)),
                new("Central", _ => Task.FromException<int>(new IOException("gateway closed")))
            };
            BatchReport result = await BatchRunner.RunAsync(jobs);
            return result.Succeeded.Count == 1 &&
                   result.Succeeded[0].Partner == "South" &&
                   result.Failed.Count == 1 &&
                   result.Failed[0].Partner == "Central" &&
                   result.Failed[0].ErrorType == "IOException";
        });

        await Check("cancellation propagates rather than becoming partner failure", async () =>
        {
            using var source = new CancellationTokenSource();
            var jobs = new FeedJob[]
            {
                new("Harbor", token =>
                {
                    source.Cancel();
                    return Task.FromCanceled<int>(token);
                })
            };
            try
            {
                await BatchRunner.RunAsync(jobs, source.Token);
                return false;
            }
            catch (OperationCanceledException)
            {
                return true;
            }
        });

        await Check("synchronous source exception attributed without aborting other sources", async () =>
        {
            var jobs = new FeedJob[]
            {
                new("Cliff", _ => throw new InvalidOperationException("parse failed")),
                new("Port", _ => Task.FromResult(8))
            };
            BatchReport result = await BatchRunner.RunAsync(jobs);
            return result.Succeeded.Count == 1 &&
                   result.Succeeded[0].Partner == "Port" &&
                   result.Failed.Count == 1 &&
                   result.Failed[0].Partner == "Cliff" &&
                   result.Failed[0].ErrorType == "InvalidOperationException";
        });

        await Check("empty batch is valid", async () =>
        {
            BatchReport result = await BatchRunner.RunAsync(Array.Empty<FeedJob>());
            return result.Succeeded.Count == 0 && result.Failed.Count == 0;
        });

        Console.WriteLine("Verification: " + passed + "/" + total + " passed.");
        return passed == total ? 0 : 1;
    }
}
