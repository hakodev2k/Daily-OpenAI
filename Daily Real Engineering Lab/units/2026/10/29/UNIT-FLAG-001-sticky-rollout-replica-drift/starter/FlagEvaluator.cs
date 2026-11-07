using System.Text;

public sealed class FlagEvaluator
{
    private readonly uint _deploymentSeed;
    public FlagEvaluator(uint deploymentSeed) => _deploymentSeed = deploymentSeed;

    public int Bucket(string feature, string tenantId, string subjectId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(feature);
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(subjectId);

        byte[] key = Encoding.UTF8.GetBytes(
            $"{feature.Length}:{feature}{tenantId.Length}:{tenantId}{subjectId.Length}:{subjectId}");
        uint hash = 2166136261u;
        foreach (byte value in key)
            hash = unchecked((hash ^ value) * 16777619u);

        return (int)((hash ^ _deploymentSeed) % 10_000u);
    }

    public bool IsEnabled(string feature, string tenantId, string subjectId, int basisPoints)
    {
        if ((uint)basisPoints > 10_000u)
            throw new ArgumentOutOfRangeException(nameof(basisPoints));
        return Bucket(feature, tenantId, subjectId) < basisPoints;
    }
}
