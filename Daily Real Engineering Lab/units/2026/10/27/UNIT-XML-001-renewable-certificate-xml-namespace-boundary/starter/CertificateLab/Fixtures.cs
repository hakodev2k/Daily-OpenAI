namespace CertificateLab;

public static class Fixtures
{
    public const string DefaultNamespace = """
        <Settlement xmlns="urn:energy:certificate:v2" xmlns:ext="urn:partner:annotations" batch="OCT-27">
          <Meter id="M-A"><Reading kwh="12.5" /></Meter>
          <Meter id="M-B"><Reading kwh="7.5" /></Meter>
          <Diagnostics><Reading kwh="9000" /></Diagnostics>
          <ext:Reading kwh="5000" />
        </Settlement>
        """;

    public const string PrefixedEquivalent = """
        <c:Settlement xmlns:c="urn:energy:certificate:v2" xmlns:e="urn:partner:annotations" batch="OCT-27">
          <c:Meter id="M-A"><c:Reading kwh="12.5" /></c:Meter>
          <c:Meter id="M-B"><c:Reading kwh="7.5" /></c:Meter>
          <c:Diagnostics><c:Reading kwh="9000" /></c:Diagnostics>
          <e:Reading kwh="5000" />
        </c:Settlement>
        """;

    public const string UnsupportedVersion = """
        <Settlement xmlns="urn:energy:certificate:v1" batch="LEGACY">
          <Meter id="M-A"><Reading kwh="12.5" /></Meter>
        </Settlement>
        """;

    public const string MissingEnergyValue = """
        <Settlement xmlns="urn:energy:certificate:v2" batch="INVALID">
          <Meter id="M-A"><Reading /></Meter>
        </Settlement>
        """;
}
