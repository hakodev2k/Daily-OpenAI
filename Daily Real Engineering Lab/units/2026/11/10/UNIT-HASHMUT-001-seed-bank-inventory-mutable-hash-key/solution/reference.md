# Reference solution — Spoiler

## Root cause
`HashSet<SeedLot>` lưu entry vào bucket dựa trên hash tại thời điểm `Add`. `SeedLot.GetHashCode` sử dụng `Shelf`, nhưng `MoveTo` thay đổi `Shelf`. Sau thay đổi, `Contains` và `Remove` tra cứu bucket mới trong khi entry vẫn ở bucket cũ. Hash collision có thể che giấu lỗi trên một runtime cụ thể; không dựa vào một giá trị hash duy nhất để chứng minh tính đúng.

## Fix
Chỉ dùng identity bất biến `LotId` cho equality và hash:

```csharp
public bool Equals(SeedLot? other) =>
    other is not null && StringComparer.Ordinal.Equals(LotId, other.LotId);
public override bool Equals(object? obj) => obj is SeedLot other && Equals(other);
public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(LotId);
```

Giữ `Shelf` mutable và `MoveTo` không đổi. Chạy `./verify.ps1` hoặc lệnh dotnet tương đương; phải thấy `contains-after=True`, `remove-after=True`, `PASS`.

## Alternatives
- `Dictionary<string, SeedLot>` keyed by immutable `LotId`: dễ lookup theo mã, hỗ trợ cập nhật metadata, nhưng cần kiểm soát duplicate ID.
- Remove rồi mutate rồi Add: có thể phù hợp nếu cần identity bao gồm vị trí, nhưng cần điều phối atomicity/concurrency và rollback nếu mutation thất bại.

## Wrong fixes
- Gọi `ToList()` trước `Contains`: chỉ che giấu invariant bị hỏng, không khôi phục hash index.
- Thêm `lock`: không sửa lỗi khi chỉ một thread cập nhật.
- Override riêng `GetHashCode` mà giữ `Equals` không tương thích: phá hợp đồng equality/hash.

## Production trade-offs
Identity nên là mã lô bất biến, còn vị trí là trạng thái. Nếu domain cho phép đổi mã lô, cần một surrogate immutable ID. Không dùng `HashCode.Combine` của mutable fields làm key. Kiểm tra cả trường hợp hai mã khác nhau cùng kệ và trường hợp move nhiều lần.
