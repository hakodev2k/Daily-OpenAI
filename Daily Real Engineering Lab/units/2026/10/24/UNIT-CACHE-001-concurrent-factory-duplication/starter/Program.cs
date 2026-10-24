using System.Collections.Concurrent;
using System.Threading;
using SchemaFactoryLab;
internal static class Program
{
    private const int Workers = 8;
    private static int Main(string[] args)
    {
        if (args.Length != 1 || (args[0] != "run" && args[0] != "reproduce" && args[0] != "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run -- run|reproduce|verify");
            return 2;
        }
        const string key = "schema:tenant-42:v3";
        int compileCount = 0;
        var cache = new SchemaCache(k =>
        {
            Interlocked.Increment(ref compileCount);
            Thread.Sleep(150); // Simulate expensive compilation; not a synchronization fix.
            return new CompiledSchema(k, Guid.NewGuid());
        });
        using var gate = new Barrier(Workers + 1);
        var results = new CompiledSchema?[Workers];
        var errors = new ConcurrentQueue<Exception>();
        var threads = new Thread[Workers];
        for (int i = 0; i < Workers; i++)
        {
            int slot = i;
            threads[i] = new Thread(() =>
            {
                try
                {
                    if (!gate.SignalAndWait(TimeSpan.FromSeconds(10)))
                        throw new TimeoutException("Worker start barrier timed out.");
                    results[slot] = cache.Get(key);
                }
                catch (Exception ex) { errors.Enqueue(ex); }
            }) { IsBackground = true };
            threads[i].Start();
        }
        if (!gate.SignalAndWait(TimeSpan.FromSeconds(10)))
        {
            Console.Error.WriteLine("FAIL: start barrier timed out.");
            return 1;
        }
        foreach (Thread thread in threads)
        {
            if (!thread.Join(TimeSpan.FromSeconds(10)))
            {
                Console.Error.WriteLine("FAIL: worker did not finish.");
                return 1;
            }
        }
        if (!errors.IsEmpty || results.Any(x => x is null))
        {
            Console.Error.WriteLine("FAIL: worker error or missing result.");
            foreach (var error in errors) Console.Error.WriteLine(error);
            return 1;
        }
        bool sameInstance = results.All(x => ReferenceEquals(x, results[0]));
        bool correctKey = results.All(x => x?.Key == key);
        int firstKeyCompiles = Volatile.Read(ref compileCount);
        Console.WriteLine($"Mode={args[0]}");
        Console.WriteLine($"WorkerCount={Workers}");
        Console.WriteLine($"FirstKeyCompileCount={firstKeyCompiles}");
        Console.WriteLine($"AllWorkersShareCachedInstance={sameInstance}");
        Console.WriteLine($"AllResultsMatchKey={correctKey}");
        if (!sameInstance || !correctKey) return 1;
        if (args[0] == "reproduce")
        {
            if (firstKeyCompiles > 1)
            {
                Console.WriteLine("REPRODUCED: duplicate expensive work for one cache key.");
                return 0;
            }
            Console.Error.WriteLine("NOT REPRODUCED: retry on a less constrained machine.");
            return 1;
        }
        if (args[0] == "verify")
        {
            var other = cache.Get("schema:tenant-42:v4");
            var again = cache.Get("schema:tenant-42:v4");
            bool valid = firstKeyCompiles == 1 && Volatile.Read(ref compileCount) == 2
                && other.Key == "schema:tenant-42:v4"
                && ReferenceEquals(other, again)
                && !ReferenceEquals(other, results[0]);
            Console.WriteLine($"TotalCompileCountAfterSecondKey={Volatile.Read(ref compileCount)}");
            Console.WriteLine($"SecondKeyContractValid={valid}");
            if (valid)
            {
                Console.WriteLine("VERIFIED: one compilation per key and independent keys.");
                return 0;
            }
            Console.Error.WriteLine("FAIL: compilation count or key isolation contract.");
            return 1;
        }
        return 0;
    }
}
