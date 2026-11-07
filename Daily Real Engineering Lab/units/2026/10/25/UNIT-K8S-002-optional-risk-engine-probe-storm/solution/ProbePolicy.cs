namespace ProbeLab;
public readonly record struct HealthSnapshot(bool ProcessHealthy, bool PrimaryStoreReachable, bool OptionalRiskEngineReachable);
public sealed class ProbePolicy
{
    public bool IsLive(HealthSnapshot s) => s.ProcessHealthy;
    public bool IsReady(HealthSnapshot s) => s.ProcessHealthy && s.PrimaryStoreReachable;
    public string ServeSession(HealthSnapshot s)
    {
        if (!s.ProcessHealthy || !s.PrimaryStoreReachable) return "503";
        return s.OptionalRiskEngineReachable ? "200-personalized" : "200-fallback";
    }
}
