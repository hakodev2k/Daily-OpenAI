using System.Text.RegularExpressions;
namespace HarborInventory;
public static class ContainerLookup
{
    public static IReadOnlyList<string> Find(IReadOnlyList<string> labels, string scannedLabel)
        => labels.Where(label => Regex.IsMatch(label, scannedLabel)).ToArray();
}
