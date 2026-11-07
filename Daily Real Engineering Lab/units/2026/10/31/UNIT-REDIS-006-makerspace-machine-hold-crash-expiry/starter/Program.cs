using Lab;

var mode = args.Length == 0 ? "--probe" : args[0];
if (args.Length > 1 || mode is not ("--probe" or "--reproduce" or "--verify"))
{
    Console.Error.WriteLine("Usage: --probe | --reproduce | --verify");
    return 2;
}

var crashClock = new ManualClock();
var crashStore = new SimulatedRedis(crashClock);
var crashInjector = new CrashInjector(crashStore) { CrashAfterNextPersist = true };
var crashService = new ReservationLeaseService(crashStore, crashInjector);

bool crashInjected = false;
try
{
    crashService.TryReserve("laser-02:10am", "request-1", TimeSpan.FromMinutes(2));
}
catch (SimulatedCrashException)
{
    crashInjected = true;
}

crashStore.RestartClient();
const string key = "hold:laser-02:10am";
bool existsAfterRestart = crashStore.Exists(key);
TimeSpan? ttlAfterRestart = crashStore.TimeToLive(key);
bool finiteTtl = ttlAfterRestart is { } ttl &&
                 ttl > TimeSpan.Zero && ttl <= TimeSpan.FromMinutes(2);
crashClock.Advance(TimeSpan.FromMinutes(3));
bool retryAfterThreeMinutes =
    crashService.TryReserve("laser-02:10am", "request-2", TimeSpan.FromMinutes(2));

var normalClock = new ManualClock();
var normalStore = new SimulatedRedis(normalClock);
var normalService = new ReservationLeaseService(normalStore, new CrashInjector(normalStore));
bool first = normalService.TryReserve("laser-03:11am", "normal-1", TimeSpan.FromMinutes(2));
bool duplicate = normalService.TryReserve("laser-03:11am", "normal-2", TimeSpan.FromMinutes(2));
bool originalPreserved = normalStore.Get("hold:laser-03:11am") == "normal-1";
normalClock.Advance(TimeSpan.FromMinutes(3));
bool reclaimed = normalService.TryReserve("laser-03:11am", "normal-3", TimeSpan.FromMinutes(2));
bool normalContract = first && !duplicate && originalPreserved && reclaimed;

Console.WriteLine($"CRASH_INJECTED={crashInjected}");
Console.WriteLine($"HOLD_EXISTS_AFTER_RESTART={existsAfterRestart}");
Console.WriteLine($"FINITE_TTL_AFTER_RESTART={finiteTtl}");
Console.WriteLine($"RETRY_AFTER_3_MINUTES={retryAfterThreeMinutes}");
Console.WriteLine($"NORMAL_CONTRACT_OK={normalContract}");

bool broken = crashInjected && existsAfterRestart && !finiteTtl &&
              !retryAfterThreeMinutes && normalContract;
bool fixedState = crashInjected && existsAfterRestart && finiteTtl &&
                  retryAfterThreeMinutes && normalContract;

if (mode == "--reproduce")
{
    Console.WriteLine(broken ? "REPRODUCTION_CONFIRMED" : "REPRODUCTION_NOT_CONFIRMED");
    return broken ? 0 : 1;
}
if (mode == "--verify")
{
    Console.WriteLine(fixedState ? "VERIFICATION_PASSED" : "VERIFICATION_FAILED");
    return fixedState ? 0 : 1;
}
return 0;
