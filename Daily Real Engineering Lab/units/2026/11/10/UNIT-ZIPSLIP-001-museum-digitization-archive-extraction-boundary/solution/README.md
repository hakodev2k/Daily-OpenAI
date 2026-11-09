# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
ZIP chứa entry có tên tương đối bất thường có thể ghi file bên ngoài thư mục import.

## Evidence
Fixture `../records.txt` tạo tệp ở thư mục cha trong starter. `Path.Combine` không bảo đảm containment.

## Root cause
Tên entry không đáng tin cậy được nối trực tiếp với destination, không kiểm tra canonical path.

## Why the fix works
Chuẩn hóa root và candidate, yêu cầu candidate nằm bên trong root với dấu phân cách thư mục, và từ chối đường dẫn không hợp lệ **trước bất kỳ thao tác ghi nào**. Duyệt preflight toàn bộ entries trước khi extract để tránh import một phần khi phát hiện entry xấu.

## Reference implementation
```csharp
using System.IO.Compression;
namespace MuseumArchive;
public static class ArchiveImporter
{
    public static void Import(string zipPath, string destination)
    {
        var root = Path.GetFullPath(destination);
        var prefix = Path.TrimEndingDirectorySeparator(root) + Path.DirectorySeparatorChar;
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        using var archive = ZipFile.OpenRead(zipPath);
        var plans = new List<(ZipArchiveEntry Entry, string Target)>();
        foreach (var entry in archive.Entries)
        {
            var target = Path.GetFullPath(Path.Combine(root, entry.FullName));
            if (!target.StartsWith(prefix, comparison))
                throw new InvalidDataException("Archive entry escapes import root");
            plans.Add((entry, target));
        }
        Directory.CreateDirectory(root);
        foreach (var (entry, target) in plans)
        {
            if (string.IsNullOrEmpty(entry.Name))
            {
                Directory.CreateDirectory(target);
                continue;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            entry.ExtractToFile(target, overwrite: true);
        }
    }
}
```

## How to verify
Chạy `pwsh ./verify.ps1` trên starter đã sửa. Kiểm tra `ESCAPED=False`, import ZIP hợp lệ thành công, và bổ sung test cho đường dẫn tuyệt đối, prefix tương tự, separator trên từng OS.

## Alternative fixes
Dùng API extraction chuẩn có kiểm soát containment; vẫn cần policy cho overwrite, quota và symlink.

## Wrong or misleading fixes
Xóa chuỗi `../` bằng replace có thể bỏ sót biến thể; chỉ kiểm tra `StartsWith(root)` có thể nhận nhầm thư mục cùng prefix; chặn mọi ZIP không đáp ứng nghiệp vụ.

## Production implications
Đặt quota số entry, kích thước giải nén, giới hạn tỷ lệ nén, xử lý symlink/reparse points, quyền ghi tối thiểu và staging atomic. Canonical string containment không tự giải quyết race với symlink trong cây thư mục bị tác nhân khác thay đổi.

## Trade-offs
Preflight giảm partial writes nhưng không thay thế staging transaction; kiểm tra toàn bộ archive tốn thêm vòng duyệt metadata.

## Senior engineer should notice
Ranh giới tin cậy nằm ở entry metadata; test phải đo tác dụng phụ ngoài root, không chỉ assert exception.
