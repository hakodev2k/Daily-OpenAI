# Reference Solution — chỉ đọc sau khi tự thử

## Symptoms
Trang thứ hai có thể lặp ID của trang thứ nhất khi một bản ghi mới được chèn vào đầu danh sách. Các timestamp bằng nhau còn khiến thứ tự không xác định.

## Evidence
Với pageSize=3, trang đầu là 101,102,103. Sau khi thêm 107 vào đầu, trang thứ hai theo offset có thể bắt đầu bằng 103. Kết quả có duplicate và thiếu ID trong tập ban đầu.

## Root cause
Offset pagination đánh chỉ số trên tập dữ liệu đang biến động. Sắp xếp chỉ theo `ObservedAt` không có tie-breaker duy nhất. Session high-water mark được tạo nhưng starter chưa dùng.

## Why the fix works
Một contract chặt chẽ dùng snapshot ID bất biến cho phiên duyệt, hoặc cursor seek với thứ tự tổng `(ObservedAt DESC, Id DESC)` và ranh giới phiên. Snapshot là lựa chọn phù hợp khi yêu cầu chính xác tuyệt đối dù dữ liệu có thể sửa/xóa trong lúc duyệt.

## Reference implementation (local bounded snapshot)
```csharp
public sealed record PageSession(long MaxObservedAt, int MaxId, int[] SnapshotIds);
public static PageSession Start(IReadOnlyCollection<Alert> source) {
    var ordered = source.OrderByDescending(x => x.ObservedAt)
        .ThenByDescending(x => x.Id).ToArray();
    return new PageSession(ordered[0].ObservedAt, ordered[0].Id,
        ordered.Select(x => x.Id).ToArray());
}
public static IReadOnlyList<Alert> ReadPage(
    IReadOnlyCollection<Alert> source, PageSession session, int pageIndex, int pageSize) {
    var byId = source.ToDictionary(x => x.Id);
    return session.SnapshotIds.Skip(checked(pageIndex * pageSize)).Take(pageSize)
        .Select(id => byId[id]).ToArray();
}
```
Để áp dụng vào starter, mở rộng `PageSession` và thay `Start`, `ReadPage` trong `starter/AlertPager.cs`. Đây là snapshot ID trong memory, thích hợp cho fixture nhỏ. Nếu dữ liệu bị xóa, code tham chiếu cần giữ cả bản sao immutable hoặc xử lý tombstone theo contract.

## How to verify
Chạy `./verify.ps1`; tất cả bốn thuộc tính unique, complete, excludesNew và deterministicOrder phải true. Kiểm tra thêm timestamp bằng nhau và trường hợp dữ liệu thay đổi.

## Alternative fixes
- Keyset pagination với `WHERE (ObservedAt,Id) < (@lastTime,@lastId)` theo sort DESC: tiết kiệm memory, tốt cho stream lớn, nhưng cần xử lý cập nhật/xóa và snapshot consistency.
- Database snapshot transaction / materialized export: mạnh hơn về consistency nhưng có chi phí storage, thời gian giữ snapshot và giới hạn retention.

## Wrong / tempting fixes
- Tăng page size: chỉ che bớt triệu chứng.
- Retry trang bị lỗi: có thể nhân đôi dữ liệu.
- `Distinct()` sau khi tải: che duplicate nhưng không khôi phục missing.
- Chỉ thêm `ThenBy(Id)`: sửa tie ordering, không sửa dịch chuyển offset.

## Production implications
Cần cursor opaque có version, TTL, giới hạn kích thước; index phù hợp `(ObservedAt DESC, Id DESC)`; theo dõi tỷ lệ duplicate, missing và số cursor hết hạn. Cân nhắc privacy khi cursor chứa dữ liệu.

## Trade-offs
Snapshot ID cho tính nhất quán mạnh nhưng tốn bộ nhớ và yêu cầu quản lý phiên; keyset rẻ hơn nhưng không tự đảm bảo snapshot khi bản ghi bị sửa/xóa.

## Senior engineer should notice
Định nghĩa rõ consistency guarantee trước khi chọn công nghệ: duyệt toàn bộ tập lúc bắt đầu, hay chỉ duyệt theo thứ tự mà không trùng khi insert mới. Hai contract này không tương đương.
