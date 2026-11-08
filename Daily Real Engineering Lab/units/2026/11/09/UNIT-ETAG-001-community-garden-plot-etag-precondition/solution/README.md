# Reference Solution — Spoiler

## Root cause
`Update` loại bỏ tiền tố `W/` trước khi so sánh, khiến weak entity-tag được đối xử như strong validator. Theo RFC 9110, `If-Match` dùng **strong comparison**. Một stale weak validator không thể cấp quyền ghi đè dù opaque tag giống nhau.

## Patch tối thiểu
Trong `starter/Program.cs`, thay:
```csharp
var normalized = ifMatch.Trim().Replace("W/", "", StringComparison.OrdinalIgnoreCase);
if (normalized != current) return (412, current);
```
bằng:
```csharp
if (ifMatch.Trim() != current) return (412, current);
```
Đây là bản sửa cho fixture một strong tag. Production phải parse danh sách entity-tags, phân biệt `*`, và thực thi đầy đủ precondition semantics theo RFC; không dùng so sánh string đơn giản cho toàn bộ HTTP grammar.

## Verify
Chạy `./verify.ps1`. Mong đợi sáu dòng PASS và exit code 0. `./reproduce.ps1` sau fix phải thất bại có chủ đích vì lỗi cũ không còn.

## Wrong fixes
- Chỉ kiểm tra ETag ở controller nhưng ghi database sau đó không atomic: vẫn có TOCTOU.
- Bỏ `If-Match` để tránh 412: chuyển thành last-write-wins.
- Chấp nhận mọi validator có cùng opaque tag: vẫn nhầm weak/strong.

## Trade-offs
Ở hệ thống nhiều instance, dùng version column/rowversion và conditional UPDATE tại database, kiểm tra affected rows; lock nội bộ trong fixture chỉ mô phỏng atomicity trong một process. Quy định rõ 428 cho thiếu precondition là policy của API, không phải bắt buộc cho mọi resource.

## Reference
RFC 9110 §8.8.3 và §13.1.1: https://www.rfc-editor.org/rfc/rfc9110.html#section-13.1.1
