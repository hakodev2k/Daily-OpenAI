# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## Symptoms

Dispatcher in log `dispatcher: completing signal`, observer bắt đầu chạy và chờ cùng một gate, nhưng producer không bao giờ in `dispatcher: signal completed` hay `dispatcher: released gate`.

## Evidence

- Dispatcher đang giữ `SemaphoreSlim`.
- `SetResult` kích hoạt continuation trước khi call trả về.
- Continuation dùng `TaskContinuationOptions.ExecuteSynchronously` và cố `Wait()` trên cùng gate.
- `SemaphoreSlim` không reentrant: continuation chờ release, còn producer chỉ release sau khi `SetResult` trả về.

## Root cause

`TaskCompletionSource` mặc định cho phép continuation chạy inline trên thread hoàn tất task. Trong flow này, completion xảy ra bên trong critical section. Continuation đồng bộ lại cần chính resource producer đang giữ, tạo dependency cycle và làm producer đứng trong `SetResult`.

## Why the fix works

Reference solution tạo `TaskCompletionSource<string>` với `TaskCreationOptions.RunContinuationsAsynchronously`. Producer chỉ publish trạng thái hoàn tất; continuation được tách khỏi call stack đang giữ gate. `SetResult` có thể trả về, `finally` release gate, sau đó observer mới acquire được gate.

## How to verify

Copy cách sửa tương đương vào `starter/Program.cs`, rồi chạy:

```powershell
./verify.ps1
```

Verifier kiểm tra learner-editable starter, không chạy reference project. Flow phải kết thúc, observer vẫn nhận `ready`, và process phải in `completed`.

## Alternative fixes

- Di chuyển completion signal ra ngoài critical section nếu business invariant cho phép. Đây thường là lựa chọn tốt vì giảm code chạy khi giữ synchronization primitive.
- Thiết kế observer theo async path (`await gate.WaitAsync()`) và tránh continuation đồng bộ. Tuy nhiên chỉ đổi `Wait()` sang `WaitAsync()` mà không xem lifecycle/ownership chưa chắc giải quyết mọi continuation coupling.
- Dùng một queue/channel để tách producer và observer nếu đây thực sự là notification pipeline nhiều sự kiện. Không nên thêm queue chỉ để né một critical-section bug nhỏ.

## Wrong or misleading fixes

- Tăng ThreadPool minimum threads: deadlock dependency không biến mất vì có thêm worker.
- Thêm timeout rồi retry `SetResult`: `SetResult` không phải external transient operation và retry có thể làm semantics khó hiểu hơn.
- Bỏ synchronization hoàn toàn: có thể hết stall nhưng phá invariant mà gate đang bảo vệ.
- Chỉ đổi `SetResult` thành `TrySetResult`: khác biệt chủ yếu là duplicate-completion behavior; continuation scheduling vẫn có thể inline.

## Production implications

Completion primitives thường nằm ở boundary giữa producer/consumer, callback adapters, socket wrappers hoặc legacy event APIs. Nếu producer hoàn tất task khi đang giữ lock/semaphore, continuation của code không thuộc producer có thể chạy ngay trong critical section. Điều này làm latency, deadlock và reentrancy trở nên khó suy luận.

## Trade-offs

`RunContinuationsAsynchronously` thêm scheduling hop nhỏ nhưng làm ownership rõ hơn và tránh để arbitrary continuation chạy trong producer call stack. Với hot path cực nhạy latency, vẫn nên đo; nhưng correctness và isolation thường quan trọng hơn micro-optimization này.

## What a Senior engineer should notice

- `TaskCompletionSource` là boundary có execution semantics, không chỉ là container cho result.
- Không gọi code có thể kích hoạt callback/continuation tùy ý trong critical section nếu không kiểm soát được execution.
- Fix tốt cần phá dependency cycle, không chỉ che timeout.
- Verification phải chứng minh observer behavior vẫn còn, không phải xóa continuation để process thoát.