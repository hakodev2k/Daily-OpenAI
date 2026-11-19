# Reference Solution — chỉ đọc sau khi thực hành

## Root cause
`Confirm` trả `IEnumerable<ManifestRow>` dựa trên chuỗi LINQ deferred. Lệnh `Where` và `Select` chưa chạy tại thời điểm xác nhận; lần duyệt trong export mới đọc `Cargo.Status` và `Cargo.WeightTonnes` đã thay đổi. Đây là lỗi về ranh giới thời điểm, không phải sai công thức cộng.

## Fix tham khảo
Trong `ManifestService.Confirm`, trả về snapshot của projection tại thời điểm xác nhận:

```csharp
return cargo.Where(x => x.Status == "Ready")
    .Select(x => new ManifestRow(x.ContainerId, x.WeightTonnes))
    .ToArray();
```

`ManifestRow` là immutable record với các giá trị được copy; chỉ `cargo.ToList()` sẽ không đủ vì đối tượng Cargo bên trong vẫn mutable. Với hệ thống thực tế, xác nhận manifest nên được persist như versioned immutable business artifact trong transaction, có idempotency key và audit trail; snapshot in-memory chỉ chứng minh cơ chế local.

## Verification
Chạy `./scripts/verify.ps1` trên code người học; kết quả mong đợi `ACTUAL=30`, `VERIFY PASS`. Chạy `./scripts/reproduce.ps1` sau khi sửa sẽ báo `NOT REPRODUCED` và script thất bại theo thiết kế: script đó chỉ dùng cho trạng thái starter ban đầu.

## Wrong fixes
- Thêm `OrderBy` khi export: không thay đổi thời điểm đọc dữ liệu.
- Chỉ gọi `cargo.ToList()`: sao chép references, không đóng băng thuộc tính.
- Khóa list trong lúc export: không bảo đảm đúng snapshot đã chốt trước đó và khó phối hợp với các writer.

## Trade-offs
Snapshot tốn bộ nhớ O(n), nhưng mang semantics rõ ràng. Với hàng triệu dòng, dùng snapshot persisted/versioned và streaming từ snapshot đó thay vì giữ toàn bộ in-memory. Cần phân biệt live operational view và confirmed document.
