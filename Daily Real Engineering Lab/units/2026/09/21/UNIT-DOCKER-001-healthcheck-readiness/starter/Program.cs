sealed class AppState
{
    public bool ProcessAlive { get; set; } = true;
    public bool InitializationComplete { get; set; }
}

static int Health(AppState state) => state.ProcessAlive ? 200 : 503;
static int BusinessRequest(AppState state) => state.InitializationComplete ? 200 : 503;

var state = new AppState();
Console.WriteLine($"t=0 health={Health(state)} business={BusinessRequest(state)}");

if (args.Contains("--reproduce"))
    return Health(state) == 200 && BusinessRequest(state) == 503 ? 0 : 2;

if (args.Contains("--verify"))
    return Health(state) == 503 ? 0 : 3;

state.InitializationComplete = true;
Console.WriteLine($"t=ready health={Health(state)} business={BusinessRequest(state)}");
return 0;