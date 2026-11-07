# UNIT-API-005 — API concurrency contract investigation

## Bối cảnh
Một API quản lý hồ sơ cho phép hai client đọc cùng một resource rồi cập nhật độc lập. Trong production, request đều trả về thành công nhưng thay đổi của một client đôi khi biến mất.

## Mục tiêu
Bạn cần tái hiện hiện tượng, thu thập evidence, xác định boundary đang thiếu trong HTTP/API contract, sửa starter và chứng minh rằng stale update không còn âm thầm ghi đè dữ liệu mới hơn.

## Chạy lab
```powershell
./scripts/reproduce.ps1
```

Sau khi điều tra và sửa code trong `starter/`:

```powershell
./scripts/verify.ps1
```

## Evidence cần thu thập
- version/ETag mà mỗi client quan sát khi đọc resource
- trạng thái resource sau từng write
- HTTP status/result của stale write
- bằng chứng rằng fix không chỉ serialize toàn bộ request trong process

## Yêu cầu
1. Giữ optimistic concurrency ở API boundary.
2. Client phải gửi điều kiện dựa trên version đã đọc.
3. Stale write phải bị từ chối rõ ràng thay vì trả success.
4. Verification phải kiểm tra observable behavior, không kiểm tra tên method hay implementation detail.

## Hints
- Hint 0: So sánh state mà client B đã đọc với state tồn tại ngay trước khi B ghi.
- Hint 1: HTTP có conditional request semantics dành cho resource versioning.
- Hint 2: Tìm hiểu `ETag`, `If-Match` và status code khi precondition không còn đúng.
- Hint 3: Fix nên biến version đã đọc thành một precondition của write, không phải chỉ là metadata để log.

> Reference solution nằm trong `solution/` và có spoiler. Chỉ mở sau khi bạn đã điều tra.