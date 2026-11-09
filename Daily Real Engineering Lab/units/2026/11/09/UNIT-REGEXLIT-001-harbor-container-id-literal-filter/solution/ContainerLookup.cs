namespace HarborInventory;
public static class ContainerLookup
{
    public static IReadOnlyList<string> Find(IReadOnlyList<string> labels, string scannedLabel)
    {
        ArgumentNullException.ThrowIfNull(labels);
        ArgumentNullException.ThrowIfNull(scannedLabel);
        return labels.Where(label => string.Equals(label, scannedLabel, StringComparison.Ordinal)).ToArray();
    }
}
