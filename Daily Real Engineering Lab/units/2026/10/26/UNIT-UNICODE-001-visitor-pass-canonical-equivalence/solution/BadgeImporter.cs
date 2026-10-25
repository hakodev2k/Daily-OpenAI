using System.Text;

namespace VisitorPassLab;

public sealed record BadgeScan(string EventId, string PassCode, string Source);

public static class BadgeImporter
{
    public static IReadOnlyList<BadgeScan> Import(IEnumerable<BadgeScan> scans)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var accepted = new List<BadgeScan>();

        foreach (var scan in scans)
        {
            // Dùng khóa chuẩn hóa chỉ để so sánh; giữ nguyên dữ liệu audit gốc.
            string identityKey = scan.PassCode.Normalize(NormalizationForm.FormC);
            if (seen.Add(identityKey))
            {
                accepted.Add(scan);
            }
        }

        return accepted;
    }
}
