namespace PermitRegistryLab;

public sealed record PermitRevision(string ExternalId, string Status);

public static class PermitRegistry
{
    // Technical ASCII IDs use the same equality rule on every host.
    public static IReadOnlyDictionary<string, PermitRevision> Reconcile(
        IEnumerable<PermitRevision> feed)
    {
        var byId = new Dictionary<string, PermitRevision>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var revision in feed)
        {
            if (string.IsNullOrWhiteSpace(revision.ExternalId))
                throw new ArgumentException("Permit ID must be present.");

            byId[revision.ExternalId] = revision;
        }

        return byId;
    }
}
