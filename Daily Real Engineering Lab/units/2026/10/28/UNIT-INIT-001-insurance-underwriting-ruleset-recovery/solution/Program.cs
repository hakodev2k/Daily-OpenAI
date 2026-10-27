using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RulesetLab;

internal sealed record Ruleset(string Version, int RuleCount);

internal interface IRulesetProvider
{
    Task<Ruleset> FetchAsync();
}

internal sealed class ScriptedRulesetProvider : IRulesetProvider
{
    private readonly bool _failFirst;
    private int _attempts;

    public ScriptedRulesetProvider(bool failFirst) => _failFirst = failFirst;

    public int Attempts => Volatile.Read(ref _attempts);

    public async Task<Ruleset> FetchAsync()
    {
        int attempt = Interlocked.Increment(ref _attempts);
        await Task.Delay(25).ConfigureAwait(false);

        if (_failFirst && attempt == 1)
            throw new HttpRequestException("Simulated partner HTTP 503");

        return new Ruleset("2026-Q4", 128);
    }
}

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        string mode = args.Length == 0 ? "run" : args[0].ToLowerInvariant();

        try
        {
            return mode switch
            {
                "run" => await RunAsync(),
                "reproduce" => await ReproduceAsync(),
                "verify" => await VerifyAsync(),
                _ => InvalidMode(mode)
            };
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("CHECK_FAILED: " + exception.GetType().Name + ": " + exception.Message);
            return 2;
        }
    }

    private static int InvalidMode(string mode)
    {
        Console.Error.WriteLine("Unknown mode: " + mode);
        return 64;
    }

    private static async Task<int> RunAsync()
    {
        var provider = new ScriptedRulesetProvider(failFirst: true);
        var cache = new RulesetSnapshotCache(provider);

        for (int i = 1; i <= 3; i++)
        {
            try
            {
                Ruleset ruleset = await cache.GetAsync();
                Console.WriteLine("REQUEST=" + i + " RESULT=OK VERSION=" + ruleset.Version);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine("REQUEST=" + i + " RESULT=503 MESSAGE=" + exception.Message);
            }
        }

        Console.WriteLine("PROVIDER_CALLS=" + provider.Attempts);
        return 0;
    }

    private static async Task<int> ReproduceAsync()
    {
        var provider = new ScriptedRulesetProvider(failFirst: true);
        var cache = new RulesetSnapshotCache(provider);

        bool firstFailed = await IsSimulated503Async(cache.GetAsync());
        bool secondFailed = await IsSimulated503Async(cache.GetAsync());
        bool observed = firstFailed && secondFailed && provider.Attempts == 1;

        Console.WriteLine("FIRST_503=" + firstFailed);
        Console.WriteLine("SECOND_503=" + secondFailed);
        Console.WriteLine("PROVIDER_CALLS=" + provider.Attempts);
        Console.WriteLine(observed ? "REPRODUCED" : "NOT_REPRODUCED");
        return observed ? 0 : 1;
    }

    private static async Task<int> VerifyAsync()
    {
        var provider = new ScriptedRulesetProvider(failFirst: true);
        var cache = new RulesetSnapshotCache(provider);

        Require(await IsSimulated503Async(cache.GetAsync()), "first fetch must reproduce 503");
        Ruleset recovered = await cache.GetAsync();
        Ruleset cached = await cache.GetAsync();

        Require(recovered.Version == "2026-Q4" && recovered.RuleCount == 128,
            "recovery must return expected ruleset");
        Require(ReferenceEquals(recovered, cached),
            "successful snapshot must be reused");
        Require(provider.Attempts == 2,
            "one failed attempt plus exactly one successful retry required");

        var concurrentProvider = new ScriptedRulesetProvider(failFirst: false);
        var concurrentCache = new RulesetSnapshotCache(concurrentProvider);
        Task<Ruleset>[] requests = Enumerable.Range(0, 12)
            .Select(_ => concurrentCache.GetAsync())
            .ToArray();
        Ruleset[] results = await Task.WhenAll(requests);

        Require(concurrentProvider.Attempts == 1,
            "concurrent cold requests must share one fetch");
        Require(results.All(result => ReferenceEquals(result, results[0])),
            "concurrent callers must observe the same snapshot");

        Console.WriteLine("VERIFY_PASSED: recovered after 503; cached success; single-flight cold load");
        return 0;
    }

    private static async Task<bool> IsSimulated503Async(Task<Ruleset> operation)
    {
        try
        {
            await operation;
            return false;
        }
        catch (HttpRequestException exception)
            when (exception.Message.Contains("503", StringComparison.Ordinal))
        {
            return true;
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
