# Reference Solution — chỉ xem sau khi reproduce và tự sửa

## Triệu chứng và bằng chứng
Hai client đọc revision=1. Alice lưu ghi chú, revision=2. Bob gửi toàn bộ snapshot revision=1; API vẫn báo SUCCESS và thay ghi chú của Alice. Log 200 chỉ chứng minh request xử lý, không chứng minh dữ liệu không mất.

## Root cause
Save thay toàn bộ document vô điều kiện. Lock bảo vệ khỏi data race trong process nhưng không bảo vệ khỏi stale writes. Lost update là lỗi hợp đồng concurrency, không phải lỗi throughput.

## Fix
Trong `ManifestStore.Save`, ngay bên trong lock, thêm:
```csharp
if (submitted.Revision != _current.Revision)
    return SaveResult.Conflict;
```
Giữ thao tác tăng revision và cập nhật trong cùng lock. Chạy `./verify.ps1` trên code learner đã sửa. Reference implementation độc lập ở [Program.cs](Program.cs).

## Vì sao đúng
Client thứ hai phải reload phiên bản mới trước khi thử cập nhật lại; không thể âm thầm ghi đè. Trong database, dùng `UPDATE ... WHERE Id=@id AND Version=@expected`, kiểm tra affected rows, và thay đổi version trong cùng transaction. HTTP có thể dùng `ETag` / `If-Match` và trả `412 Precondition Failed`; `409 Conflict` cũng có thể phù hợp tùy API contract.

## Alternative fixes và trade-offs
Field-level patch hoặc merge theo domain có thể giữ cả hai chỉnh sửa nếu độc lập, nhưng cần định nghĩa rõ xung đột khi cùng field thay đổi. Pessimistic locking hạn chế conflict nhưng tăng blocking và chi phí vận hành. Event sourcing giữ lịch sử nhưng không thay thế concurrency control.

## Wrong / tempting fixes
Tăng timeout, retry tự động với snapshot cũ, hoặc chỉ thêm lock quanh unconditional replacement đều không xử lý stale data. Last-write-wins có thể là policy hợp lệ với dữ liệu không quan trọng, nhưng không phù hợp manifest có audit.

## Production implications
Ghi audit revision, actor, conflict count; không log dữ liệu cá nhân không cần thiết. Xử lý version token không đoán được nếu có yêu cầu bảo mật. Nếu có nhiều instance, in-process lock không đủ: dùng conditional DB update. Kiểm thử hai phiên cùng version và retry sau reload.
