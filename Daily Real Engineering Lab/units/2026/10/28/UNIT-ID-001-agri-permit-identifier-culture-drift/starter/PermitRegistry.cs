namespace PermitRegistryLab;

public sealed record PermitRevision(string ExternalId, string Status);

public static class PermitRegistry
{
    // The partner sends multiple revisions in arrival order; last revision wins.
    public static IReadOnlyDictionary<string, PermitRevision> Reconcile(
        IEnumerable<PermitRevision> feed)
    {
        var byId = new Dictionary<string, PermitRevision>(
            StringComparer.CurrentCultureIgnoreCase);

        foreach (var revision in feed)
        {
            if (string.IsNullOrWhiteSpace(revision.ExternalId))
                throw new ArgumentException("Permit ID must be present.");

            byId[revision.ExternalId] = revision;
        }

        return byId;
    }
}
