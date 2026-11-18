using System;
using System.Collections.Generic;

var manifests = new[]
{
    new Container("VN-HCM", "CXRU-412"),
    new Container("vn-hcm", "cxru-412"),
    new Container("VN-DAD", "ABCU-901")
};
var comparer = new ContainerComparer();
var unique = new HashSet<Container>(comparer);
foreach (var manifest in manifests) unique.Add(manifest);
var equivalent = comparer.Equals(manifests[0], manifests[1]);
var hashEqual = comparer.GetHashCode(manifests[0]) == comparer.GetHashCode(manifests[1]);
Console.WriteLine($"equivalent={equivalent}; hashEqual={hashEqual}; unique={unique.Count}; expected=2");
Console.WriteLine($"lookup={unique.Contains(new Container("vn-hcm", "CXRU-412"))}");
var valid = unique.Count == 2 && unique.Contains(new Container("vn-hcm", "CXRU-412")) && (!equivalent || hashEqual);
if (args.Contains("--verify")) return valid ? 0 : 1;
return valid ? 0 : 1;

public sealed record Container(string PortCode, string ContainerNumber);

public sealed class ContainerComparer : IEqualityComparer<Container>
{
    public bool Equals(Container? x, Container? y) =>
        ReferenceEquals(x, y) || (x is not null && y is not null &&
        StringComparer.OrdinalIgnoreCase.Equals(x.PortCode, y.PortCode) &&
        StringComparer.OrdinalIgnoreCase.Equals(x.ContainerNumber, y.ContainerNumber));

    public int GetHashCode(Container obj) =>
        HashCode.Combine(StringComparer.OrdinalIgnoreCase.GetHashCode(obj.PortCode), StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ContainerNumber));
}
