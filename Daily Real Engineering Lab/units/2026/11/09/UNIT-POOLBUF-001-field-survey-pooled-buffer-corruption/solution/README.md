# Reference solution — spoiler

## Root cause
`SubmitAsync` trả về `Task<byte[]>` của `StoreLaterAsync` mà không đợi nó hoàn tất bên trong phạm vi sở hữu buffer. `finally` trả mảng về `ArrayPool` ngay khi task còn đang chờ `storageGate`. Một renter khác có thể nhận cùng mảng và ghi đè dữ liệu. `ArrayPool` không bảo đảm zeroing hay tính độc quyền sau `Return`.

## Fix
Giữ quyền sở hữu rented buffer cho tới khi consumer hoàn tất: đổi `SubmitAsync` thành `async Task<byte[]>` và `return await StoreLaterAsync(...)` bên trong `try`; `finally` trả buffer đúng một lần cả khi exception/cancellation.

## Evidence
Lab dùng `DeterministicPool` mô phỏng hợp đồng thuê/trả để tái hiện chắc chắn, không giả định hành vi tái sử dụng cụ thể của `ArrayPool.Shared`. Ở starter, log `competing workload` xuất hiện trước khi gate mở và dữ liệu đọc ra khác `0A141E28`. Reference giữ buffer trong lúc chờ nên renter cạnh tranh không nhận mảng đang dùng.

## Wrong fixes
- `clearArray: true` khi trả buffer: chỉ làm mất dữ liệu sớm hơn, không bảo vệ consumer.
- `Task.Delay(100)`: phụ thuộc timing, không tạo ownership.
- Thêm `lock` quanh lệnh `Return`: không bao trùm quá trình đọc bất đồng bộ.
- Tắt pooling toàn hệ thống: có thể tránh symptom nhưng tăng allocation/GC; chưa xác định trade-off.

## Trade-offs
Một lựa chọn khác là copy payload sang mảng sở hữu riêng trước khi trả buffer; đơn giản về lifetime nhưng tăng allocation. Khi dùng buffer pool, phải xác định rõ thời điểm hoàn thành của mọi consumer, exception path và cancellation.

## Verification
`./scripts/verify.ps1` chạy chính starter đã chỉnh sửa 10 lần; exit code 0 và không có mismatch mới là PASS.
