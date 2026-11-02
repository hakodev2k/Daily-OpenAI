# Reference Solution — SPOILER

Chỉ đọc sau khi đã tự reproduce, viết hypothesis, thử fix và chạy verification.

## 1. Symptoms
Worker nhập ZIP thành công nhưng xuất hiện file ngoài `extract/`; happy-path import vẫn hoạt động.

## 2. Evidence
Local fixture có entry hợp lệ và entry bất thường. `reproduce.ps1` chứng minh tệp nằm ở parent của root. Negative test sibling-prefix phát hiện trường hợp hai đường dẫn có cùng prefix tên nhưng khác directory.

## 3. Root cause
Starter tin `ZipArchiveEntry.FullName` và ghép trực tiếp với destination, không chuẩn hóa và xác thực đường dẫn sau khi resolve. Entry có thể điều khiển output ra ngoài root.

## 4. Why the fix works
Reference code chuẩn hóa separator, từ chối đường dẫn tuyệt đối, tính `Path.GetFullPath`, xác thực output bắt đầu bằng root **kèm directory separator**, kiểm tra toàn bộ entries trước khi ghi, từ chối path trùng và không overwrite.

## 5. How to verify
Chỉ sau khi tự sửa, so sánh `solution/ArchiveImportService.cs` với `starter/ArchiveImportService.cs`. `./verify.ps1` luôn kiểm tra project `starter/`, không tự dùng solution. `./reproduce.ps1` được thiết kế để thành công trên starter lỗi và sẽ không còn thành công sau khi fix.

## 6. Alternative fixes
Sử dụng API extraction đã có traversal protection và kiểm thử theo hợp đồng; hoặc chỉ chấp nhận allowlist cấu trúc file. Với yêu cầu publish nguyên tử, extract vào staging cô lập rồi publish.

## 7. Wrong / tempting fixes
- Xóa chuỗi `../`: không bao quát mọi separator và nested traversal.
- `StartsWith(root)` thiếu separator: `extract-sibling` vẫn qua.
- Chỉ lọc extension: không xác thực vị trí output.
- Catch rồi bỏ qua lỗi: có thể che giấu partial write.
- Tăng quyền hay retry: không xử lý trust boundary.

## 8. Production implications
Bài lab chưa xử lý đầy đủ ZIP bomb, số lượng entry, kích thước uncompressed, symlink/reparse point, TOCTOU, quota, malware scanning và atomic staging. Không giải nén input không tin cậy vào thư mục có quyền rộng hoặc chứa dữ liệu production.

## 9. Trade-offs
Preflight two-pass ngăn partial write do tên entry không hợp lệ nhưng không phải transaction nếu disk full hoặc I/O lỗi. Staging + atomic publish tăng disk I/O, đổi lại isolation và rollback rõ ràng.

## 10. Senior engineer should notice
Phải định nghĩa invariant, thu thập evidence, test cả negative và positive path, phân biệt security boundary với operational failure, và nêu rủi ro còn lại sau fix.
