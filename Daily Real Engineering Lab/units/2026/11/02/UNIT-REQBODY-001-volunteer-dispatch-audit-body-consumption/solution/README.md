# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Bật audit middleware khiến JSON hợp lệ bị HTTP 400; audit vẫn thấy payload. Payload JSON sai cũng HTTP 400, nên chỉ nhìn status không phân biệt được nguồn lỗi.

## Evidence
\`--reproduce\` cho thấy audit có dữ liệu nhưng endpoint không nhận được ticket hợp lệ. Request dùng stream forward-only để phản ánh đặc tính mạng. Endpoint deserialize sau middleware.

## Root cause
Middleware đã đọc toàn bộ \`HttpRequest.Body\` trước khi chuyển tiếp. Stream body có trạng thái đọc và không tự tua lại; endpoint đọc từ vị trí cuối, gặp EOF rồi \`JsonException\`. Lỗi không nằm ở schema JSON.

## Why this fix works
Trước khi đọc, gọi \`HttpRequest.EnableBuffering\` với giới hạn dung lượng phù hợp. Sau khi audit hoàn thành, đặt \`Request.Body.Position = 0\` rồi mới gọi \`_next(context)\`. \`StreamReader\` dùng \`leaveOpen: true\` để không đóng body thuộc sở hữu request. Đối với body lớn hơn giới hạn, hệ thống production cần chính sách trả 413 rõ ràng, không nuốt \`IOException\`.

## How to verify
Trong thư mục unit, sửa \`starter/Program.cs\` theo cách của bạn và chạy:
~~~powershell
./verify.ps1
~~~
Contract bắt buộc: JSON hợp lệ trả 200 cùng \`accepted:V-104\`; JSON sai trả 400; cả hai tạo audit fingerprint không rỗng và khác nhau. Script kiểm tra **starter bạn đã sửa**, không kiểm tra solution.

Để tự so sánh reference (chỉ sau khi thử), xem \`solution/Program.cs\`. Có thể sao chép riêng file này vào \`starter/Program.cs\` sau khi lưu bản tự làm để đối chiếu.

## Alternative fixes
- Thiết kế audit ở endpoint/filter sau model binding, nhưng khi đó không quan sát chính xác raw payload của request lỗi.
- Dùng middleware đọc theo chunks với giới hạn và stream replay riêng: linh hoạt hơn, nhưng tăng chi phí code và rủi ro quản lý vòng đời.
- Nếu không cần đọc body, chỉ audit metadata/trace ID: đơn giản và ít rủi ro dữ liệu hơn.

## Wrong / tempting fixes
- Tăng timeout hoặc retry webhook: không thay đổi vị trí đọc của body.
- Bắt \`JsonException\` và trả 200: che giấu lỗi và có thể chấp nhận ticket không hợp lệ.
- Chỉ \`Position = 0\` trên network stream gốc: stream không seekable, có thể ném \`NotSupportedException\`.
- Tạo \`StreamReader\` mới mà không reset: reader mới vẫn đọc từ vị trí hiện tại.
- \`Dispose\` body để “dọn tài nguyên”: làm hỏng consumer downstream.

## Production implications
Áp giới hạn request size, tránh ghi raw body có PII/secrets, tính chi phí buffer memory/disk và xử lý client cancellation. Đặt audit middleware đúng vị trí trong pipeline, bảo đảm log failure path mà không biến lỗi nội bộ thành 200.

## Trade-offs
Buffering giúp nhiều consumer đọc lại nhưng tăng allocation/I/O, đặc biệt khi spill ra disk. Đọc metadata thay vì payload giảm overhead nhưng không chứng minh raw payload fingerprint. Với webhook bảo mật cao, cân nhắc HMAC trên raw bytes và một điểm sở hữu body rõ ràng.

## What a Senior engineer should notice
HTTP 400 có thể do trạng thái stream chứ không phải validation. Cần chứng minh request còn đọc được tại endpoint, kiểm tra cả success/error path, giới hạn tài nguyên và không vô tình log nội dung nhạy cảm.
