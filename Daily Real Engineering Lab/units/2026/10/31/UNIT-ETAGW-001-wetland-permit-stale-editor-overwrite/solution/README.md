# Reference Solution — SPOILER
Chỉ mở sau khi reproduce, ghi giả thuyết và tự sửa starter/.

## Symptoms
A và B đọc cùng ETag; hai PUT đều trả 200; bản cuối là B và ghi chú A biến mất. Không có exception.

## Evidence
GET A/B cùng "rev-1". PUT A tạo revision 2; PUT B vẫn gửi "rev-1" nhưng vẫn thành công. Audit ghi nhận hai success response trong khi business yêu cầu không mất chỉnh sửa đã chấp nhận.

## Root cause
Starter nhận tham số ifMatch nhưng ghi vô điều kiện. Validator phía client không có tác dụng nếu server không enforce precondition ở write boundary.

## Why the fix works
[Program.cs](Program.cs) kiểm tra thiếu If-Match (428), so sánh một strong ETag với revision hiện tại (không khớp: 412), rồi mới ghi và tăng revision. Check và mutation ở trong cùng lock để tránh TOCTOU trong một process. Demo chỉ hỗ trợ **một strong ETag**; HTTP API thực tế phải parse đúng danh sách ETag, wildcard và cú pháp theo RFC 9110.

## How to verify
Sửa starter/Program.cs rồi chạy ./verify.ps1. Test trên **starter** kiểm tra: ghi đầu 200; stale write 412; dữ liệu A không đổi; thiếu precondition 428; weak tag bị từ chối; đọc ETag mới và ghi tiếp 200.
Đối chiếu reference độc lập:
~~~powershell
dotnet run --project ./solution/PermitRevisionLab.csproj -- verify
~~~
Lệnh này không thay thế ./verify.ps1.

## Alternative fixes
- SQL Server rowversion / PostgreSQL version column: UPDATE ... WHERE Id = ? AND Version = ?; kiểm tra affected rows; CAS tại database boundary.
- EF Core concurrency token: bắt DbUpdateConcurrencyException và map thành 412, cung cấp dữ liệu mới để người dùng merge.
- Pessimistic locking có thể phù hợp với quy trình độc quyền sửa nhưng tăng contention và vận hành.

## Wrong / tempting fixes
- Chỉ disable Save trên UI: không ngăn client khác, refresh hoặc retry.
- So sánh version ngoài lock rồi ghi sau: race giữa check và update.
- Tự động retry stale PUT: có thể lặp lại ghi đè nếu không reload/merge.
- Thêm Redis lock mặc định: không thay thế optimistic version check tại nguồn dữ liệu.
- Last-write-wins: chỉ phù hợp khi nghiệp vụ chấp nhận mất ghi.

## Production implications
- Log documentId, actor, priorRevision, newRevision và status; tránh log nội dung nhạy cảm.
- Khi 412, UI cần reload/diff/merge, không báo “đã lưu” sai.
- ETag phải thay đổi khi representation liên quan thay đổi; xác định rõ phạm vi version.
- Lock in-memory không bảo vệ nhiều instance; cần database CAS/transaction.
- Bổ sung HTTP integration tests cho header parsing, status mapping, persistence và concurrency thật.

## Trade-offs
Optimistic concurrency có overhead thấp khi xung đột hiếm nhưng cần UX xử lý conflict. Lock dài hạn có thể giảm conflict nhưng làm tăng deadlock/timeout và độ phức tạp.

## What a Senior engineer should notice
Hai response thành công vẫn có thể vi phạm tính toàn vẹn dữ liệu. Monitoring theo error rate không đủ. Cần kiểm chứng invariant tại durable write boundary, không chỉ trong UI hay test in-memory.

## References
- [RFC 9110, If-Match](https://www.rfc-editor.org/rfc/rfc9110.html#section-13.1.1)
- [RFC 6585, 428 Precondition Required](https://www.rfc-editor.org/rfc/rfc6585.html#section-3)
