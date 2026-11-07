using System.Collections.Generic;

namespace BridgeBriefingLab;

public readonly record struct BriefingId(string TenantId, string ReportId);

public sealed class BriefingCache
{
    private readonly IReadOnlyDictionary<BriefingId, string> _origin;
    private readonly Dictionary<string, string> _cache = new(StringComparer.Ordinal);

    public int OriginReads { get; private set; }
    public int CacheHits { get; private set; }
    public int CacheEntries => _cache.Count;

    public BriefingCache(IReadOnlyDictionary<BriefingId, string> origin)
    {
        _origin = origin;
    }

    public string Get(string tenantId, string reportId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(reportId);

        string key = tenantId + ":" + reportId;

        if (_cache.TryGetValue(key, out string? cached))
        {
            CacheHits++;
            return cached;
        }

        OriginReads++;
        string briefing = _origin[new BriefingId(tenantId, reportId)];
        _cache[key] = briefing;
        return briefing;
    }
}
