# Reference Solution — chỉ đọc sau khi tự thử

## Symptoms và Evidence

Admission ghi `ACCEPTED` cho một hồ sơ mà downstream PDF parser từ chối. Hồ sơ đối chứng cùng endpoint được xử lý bình thường; không có bằng chứng network retry hay thay đổi deployment. Cần so sánh nội dung byte, không chỉ log thuộc tính request.

## Root cause

`starter/UploadGate.cs` chỉ kiểm tra tên tệp, `ContentType` do client khai báo và độ dài. Client có thể gán `.pdf` và `application/pdf` cho dữ liệu không phải PDF; những thuộc tính đó không chứng minh nội dung là PDF.

## Why the fix works

`solution/UploadGate.cs` thêm bước kiểm tra signature `%PDF-` trên byte thực tế sau khi kiểm tra giới hạn kích thước. Payload giả dạng và header thiếu không qua admission; các hợp đồng cũ vẫn được giữ. Đây chỉ là admission filter, không chứng minh toàn bộ tài liệu hợp lệ.

## How to verify

Chép logic của reference vào `starter/UploadGate.cs` sau khi tự làm, rồi chạy `pwsh ./scripts/verify.ps1`. `verify.ps1` luôn compile `starter/`, không compile file solution. `reproduce.ps1` dùng baseline độc lập để giữ phép tái hiện ổn định.

## Alternative fixes

- Sử dụng thư viện PDF parser đã được kiểm định để kiểm tra cấu trúc sâu hơn; chi phí CPU/bộ nhớ và bề mặt tấn công cần được kiểm soát.
- Quarantine file và parse ở isolated worker, chỉ publish sau khi validation hoàn tất; phù hợp pipeline có yêu cầu bảo mật cao.
- Kiểm tra signature nhẹ tại admission, rồi validation sâu ở worker; thường là phương án cân bằng cho hệ thống thực tế.

## Wrong / tempting fixes

- Chỉ tin `ContentType`: trường do client gửi, không đủ tin cậy.
- Chỉ kiểm tra `.pdf`: đổi tên file không làm thay đổi nội dung.
- Retry parser vô hạn: không biến dữ liệu sai thành hợp lệ, gây lãng phí queue.
- Tìm `%%EOF` như một chứng nhận tuyệt đối: một chuỗi marker không thay thế việc parse cấu trúc và policy.

## Production implications và Trade-offs

Giới hạn size trước khi đọc toàn bộ file; stream vào vùng cách ly, giới hạn CPU/memory, áp dụng malware scanning, tránh public file path, lưu audit reason codes và rate-limit. PDF signature là bộ lọc sơ bộ; một file có prefix hợp lệ vẫn có thể lỗi cấu trúc hoặc chứa nội dung độc hại. Quyết định reject/queue/quarantine dựa trên threat model, throughput và SLA.

## Senior engineer should notice

Phải xác định trust boundary giữa client metadata, byte payload và downstream parser; test phải kiểm chứng cả false positive lẫn false negative. Đừng đánh đồng admission validation với tài liệu an toàn hay parse được.

## References

- Microsoft Learn: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads
