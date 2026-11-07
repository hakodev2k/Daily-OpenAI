using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

public sealed class FlagEvaluator
{
    // Keep the constructor contract, but never use per-replica state in cohort assignment.
    public FlagEvaluator(uint deploymentSeed) => _ = deploymentSeed;

    public int Bucket(string feature, string tenantId, string subjectId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(feature);
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(subjectId);

        byte[][] segments =
        {
            Encoding.UTF8.GetBytes(feature),
            Encoding.UTF8.GetBytes(tenantId),
            Encoding.UTF8.GetBytes(subjectId)
        };
        byte[] payload = new byte[12 + segments.Sum(s => s.Length)];
        int offset = 0;
        foreach (byte[] segment in segments)
        {
            BinaryPrimitives.WriteInt32BigEndian(payload.AsSpan(offset, 4), segment.Length);
            offset += 4;
            segment.CopyTo(payload, offset);
            offset += segment.Length;
        }
        byte[] digest = SHA256.HashData(payload);
        ulong hash = BinaryPrimitives.ReadUInt64BigEndian(digest.AsSpan(0, 8));
        return (int)(hash % 10_000UL);
    }

    public bool IsEnabled(string feature, string tenantId, string subjectId, int basisPoints)
    {
        if ((uint)basisPoints > 10_000u)
            throw new ArgumentOutOfRangeException(nameof(basisPoints));
        return Bucket(feature, tenantId, subjectId) < basisPoints;
    }
}
