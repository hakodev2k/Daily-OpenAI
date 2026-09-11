# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Batch có 3 item: item đầu insert được, item thứ hai vi phạm unique constraint và được application catch, nhưng item thứ ba hợp lệ vẫn fail với SQLSTATE `25P02`. Kết quả cuối không thể commit hai customer distinct như yêu cầu.

## 2. Evidence

Evidence quan trọng là chuỗi SQLSTATE:

- duplicate: `23505` (`unique_violation`)
- command hợp lệ tiếp theo: `25P02` (`in_failed_sql_transaction`)

Điều này cho thấy application đã catch exception nhưng database transaction vẫn ở failed state.

## 3. Root cause

Trong PostgreSQL, khi một statement bên trong transaction fail, transaction hiện tại chuyển sang aborted/failed state. `try/catch` của C# chỉ thay đổi control flow ở application; nó không tự phục hồi database transaction. Các command tiếp theo bị từ chối cho đến khi transaction được rollback, hoặc application đã thiết kế một recovery boundary phù hợp như savepoint.

Starter đang dùng exception cho một outcome được business xem là bình thường: duplicate customer. Vì vậy một expected conflict lại làm hỏng transaction chứa cả batch.

## 4. Why the fix works

Reference solution chuyển duplicate policy xuống SQL bằng:

```sql
ON CONFLICT (external_id) DO NOTHING
```

Duplicate không còn là statement error. PostgreSQL trả affected-row count bằng `0`, transaction vẫn usable, item tiếp theo vẫn chạy và toàn batch có thể commit.

## 5. How to verify

Sau khi áp dụng cùng contract vào `starter/`, chạy:

```powershell
./verify.ps1
```

Kết quả cần có:

- không còn `25P02`
- `BATCH_COMMITTED=true`
- `ROW_COUNT=2`

## 6. Alternative fixes

### Savepoint cho từng item

Có thể tạo savepoint trước mỗi insert và `ROLLBACK TO SAVEPOINT` khi gặp duplicate. Cách này hữu ích khi expected error không thể biểu diễn sạch bằng `ON CONFLICT`, nhưng phức tạp hơn và tăng round trip/transaction bookkeeping.

### Tách transaction theo item

Có thể cho mỗi row một transaction riêng, nhưng làm thay đổi atomicity của batch. Chỉ phù hợp nếu nghiệp vụ chấp nhận partial commit độc lập.

## 7. Wrong or misleading fixes

### Chỉ catch `23505` rồi `continue`

Đây chính là starter. Application tiếp tục, nhưng PostgreSQL transaction không tự hồi phục.

### `SELECT` trước rồi mới `INSERT`

Có thể giảm duplicate trong demo đơn luồng nhưng tạo check-then-act race dưới concurrency. Unique constraint vẫn phải là authority cuối cùng.

### Retry command tiếp theo trong cùng failed transaction

Retry không thay đổi transaction state nên vẫn nhận `25P02`.

### Nuốt mọi `PostgresException`

Điều này che mất lỗi thật như connectivity, invalid SQL hoặc constraint khác và có thể tạo false-success.

## 8. Production implications

Expected business conflicts nên được mô hình hóa rõ ràng thay vì biến thành generic exception flow nếu database cung cấp primitive phù hợp. Điều này làm transaction semantics dễ reasoning hơn, giảm noise trong telemetry và tránh batch failure dây chuyền.

## 9. Trade-offs

`ON CONFLICT DO NOTHING` phù hợp khi duplicate thực sự có nghĩa là “skip”. Nếu cần xác nhận payload mới có cùng dữ liệu với record cũ, hoặc cần merge/update, contract phải mạnh hơn (`DO UPDATE`, compare fingerprint, hoặc explicit validation). Đừng dùng `DO NOTHING` để che conflict có ý nghĩa nghiệp vụ khác.

## 10. What a Senior engineer should notice

Senior engineer nên tách ba layer reasoning:

1. application exception handling
2. database transaction state
3. business conflict policy

Một exception được catch không có nghĩa resource bên dưới đã trở lại trạng thái hợp lệ. Khi một failure được xem là expected outcome, thiết kế nên dùng primitive thể hiện trực tiếp outcome đó và vẫn bảo vệ invariant bằng constraint ở database.
