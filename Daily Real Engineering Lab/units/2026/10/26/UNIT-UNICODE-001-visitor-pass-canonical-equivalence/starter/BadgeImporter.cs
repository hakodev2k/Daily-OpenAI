namespace VisitorPassLab;

public sealed record BadgeScan(string EventId, string PassCode, string Source);

public static class BadgeImporter
{
    // Chính sách: một mã vé được ghi nhận theo lần quét hợp lệ đầu tiên.
    // Các sự kiện về sau có cùng định danh vé phải bị loại khỏi báo cáo lượt vào.
    public static IReadOnlyList<BadgeScan> Import(IEnumerable<BadgeScan> scans)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var accepted = new List<BadgeScan>();

        foreach (var scan in scans)
        {
            if (seen.Add(scan.PassCode))
            {
                accepted.Add(scan);
            }
        }

        return accepted;
    }
}
