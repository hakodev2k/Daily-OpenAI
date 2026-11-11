# Reference solution (spoiler)

## Root cause
Hai client đọc cùng revision 1. Save của client A tăng revision lên 2, nhưng Save của client B vẫn thay thế toàn bộ record từ revision 1; trường Copies bị đưa về 3. Đây là lost update, không phải lỗi thứ tự thread.

## Fix
Trong `Catalog.Save`, thay body bằng:

```csharp
lock (_gate)
{
    if (incoming.Revision != _book.Revision) return false;
    _book = incoming with { Revision = _book.Revision + 1 };
    return true;
}
```

Thêm `private readonly object _gate = new();` vào class Catalog; dùng cùng lock cho Read nếu có concurrent threads thực sự. Chạy `./verify.ps1` sau khi sửa. Trong HTTP thực tế, GET trả strong ETag và PUT yêu cầu If-Match; stale ETag phải trả 412, không âm thầm ghi đè. Với API đòi bắt buộc precondition, thiếu If-Match có thể trả 428.

## Wrong fixes
- Retry PUT mù: vẫn có thể ghi đè dữ liệu mới.
- Dùng last-write-wins: hợp lệ chỉ khi business chấp nhận mất cập nhật.
- Lock chỉ ở client: không bảo vệ giữa các máy khác nhau.

## Trade-offs
Optimistic concurrency phù hợp khi xung đột ít; người dùng cần UI merge/reload khi gặp 412. Đối với nhiều instance, kiểm tra phiên bản phải atomic ở database, không dựa vào process-local lock.
