# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Khi hai host cùng nhận một schedule window, cả hai đều chạy reconciliation và tạo hai business executions.

## 2. Evidence

Log cho thấy `host-a` và `host-b` đều START/DONE cùng window. Guard của mỗi job không hề reject host còn lại.

## 3. Root cause

`SemaphoreSlim` là process-local coordination. Mỗi host sở hữu một semaphore riêng, nên nó chỉ ngăn overlap bên trong chính host đó; nó không tạo mutual exclusion giữa nhiều host instance.

## 4. Why the fix works

Reference simulation đưa quyền acquire vào một shared lease store keyed theo logical schedule window. Hai host cạnh tranh trên cùng một coordination record, nên chỉ một host được phép thực hiện work cho window đó.

## 5. How to verify

`verify.ps1` kiểm tra hai điều: competing hosts chỉ tạo một execution cho window đầu, và một window mới vẫn có thể chạy đúng một lần.

## 6. Alternative fixes

Có thể dùng storage-backed lease, database row/unique constraint, hoặc một scheduler/trigger có documented singleton coordination. Nếu operation tự idempotent theo reconciliation key, duplicate invocation cũng có thể trở nên vô hại.

## 7. Wrong or misleading fixes

Tăng `SemaphoreSlim` timeout không mở rộng scope của lock. Giảm số instance về 1 che triệu chứng nhưng loại bỏ scale/failover. Dùng `static` lock vẫn chỉ có tác dụng trong một process. Chỉ kiểm tra “đã chạy chưa” rồi ghi sau work có thể tạo check-then-act race.

## 8. Production implications

Distributed lease cần timeout/expiry, ownership token, crash recovery và clock assumptions rõ ràng. Nếu reconciliation có external side effects, lease alone chưa chắc đủ; cần xem xét idempotency của chính side effect.

## 9. Trade-offs

Shared coordination thêm dependency và failure mode. Database uniqueness có thể đơn giản nếu database đã là system of record; dedicated lease phù hợp hơn khi cần ownership có thời hạn nhưng tăng operational complexity.

## 10. What a Senior engineer should notice

Concurrency control phải có scope tương ứng với deployment topology. Một primitive đúng trong single-process model có thể hoàn toàn không bảo vệ invariant khi scale-out. Cần phân biệt trigger duplication, execution coordination và business idempotency.