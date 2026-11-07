sealed class AppState { public bool ProcessAlive { get; set; } = true; public bool InitializationComplete { get; set; } }
static int Liveness(AppState state) => state.ProcessAlive ? 200 : 503;
static int Readiness(AppState state) => state.ProcessAlive && state.InitializationComplete ? 200 : 503;
static int BusinessRequest(AppState state) => state.InitializationComplete ? 200 : 503;
var state = new AppState();
Console.WriteLine($"startup live={Liveness(state)} ready={Readiness(state)} business={BusinessRequest(state)}");
state.InitializationComplete = true;
Console.WriteLine($"ready live={Liveness(state)} ready={Readiness(state)} business={BusinessRequest(state)}");