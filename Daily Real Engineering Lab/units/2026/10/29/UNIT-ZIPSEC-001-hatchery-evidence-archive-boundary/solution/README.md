# Reference Solution — Spoiler

## Symptoms
Worker báo success, nhưng file xuất hiện ngoài intake. Retry=0 và CPU thấp làm giả thuyết race/performance kém thuyết phục.

## Evidence và root cause
Starter tin tưởng entry.FullName khi tạo target path. ZIP entry có thể chứa thành phần điều hướng thư mục. Path.Combine không kiểm tra target nằm dưới root. Việc ghi tuần tự còn tạo partial extraction trước khi phát hiện entry không hợp lệ.

## Why fix works
[ArchiveIntake.cs](ArchiveIntake.cs) dùng Path.GetFullPath trên root và target, kiểm tra containment theo separator + OS comparison, từ chối tên entry không hỗ trợ và **preflight tất cả entries trước khi ghi**. Cách này đáp ứng negative checks và giữ hỗ trợ thư mục con hợp lệ.

## Verify
Sau khi tự sửa, chạy pwsh ./verify.ps1. Có thể tạm sao lưu starter/ArchiveIntake.cs, thay bằng solution/ArchiveIntake.cs để đối chiếu 5 checks rồi khôi phục bài làm. Không chỉnh checks/.

## Alternative fixes
API trích xuất cấp cao của .NET có traversal protections; vẫn cần policy của ứng dụng, quota và regression tests. Staging directory + atomic publish phù hợp hệ thống cần rollback tốt hơn.

## Wrong / tempting fixes
Chỉ replace ../ bỏ sót biến thể; StartsWith(root) thiếu separator có thể nhận nhầm thư mục anh em; kiểm tra sau khi ghi quá muộn; overwrite:false không ngăn tạo file ngoài root; chỉ dựa vào quyền OS không thay thế validation.

## Production implications và trade-offs
Preflight tốn O(n) metadata và không đảm bảo atomicity khi disk lỗi trong pha ghi. Bản mẫu giả định destination mới và đáng tin, không chứa symlink/reparse point do actor khác tạo. Production phải giới hạn bytes giải nén, số entry, duplicate/case collision, chống zip bomb, no-follow/symlink, malware scan, audit log và cleanup.

## Senior insight
Trust boundary nằm ở metadata của archive. Chứng minh bất biến không ghi ngoài root, từ chối toàn bộ gói khi policy fail, kiểm tra sibling-prefix và phân biệt policy preflight với atomic publish.
