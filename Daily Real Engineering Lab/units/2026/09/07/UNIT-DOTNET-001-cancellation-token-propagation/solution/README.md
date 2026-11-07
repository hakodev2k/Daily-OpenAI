# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Caller phát cancellation sau khoảng 150 ms nhưng starter vẫn chờ downstream hoàn thành khoảng 2 giây.

## 2. Evidence

`LoadCustomerProfileAsync` nhận `CancellationToken`, nhưng `FetchDownstreamProfileAsync` không nhận token và `Task.Delay` không quan sát token.

## 3. Root cause

Cancellation trong .NET là cooperative. `Cancel()` hoặc `CancelAfter()` không cưỡng chế dừng `Task`; nó chỉ phát tín hiệu qua token. Nếu token không được propagate tới asynchronous operation, work tiếp tục bình thường.

## 4. Why the fix works

Reference solution truyền token xuyên suốt call chain và đưa token vào `Task.Delay`. Khi caller cancel, `Task.Delay` kết thúc bằng `OperationCanceledException` thay vì giữ operation sống tới timeout tự nhiên.

## 5. How to verify

```powershell
./verify.ps1
```

Starter phải có `Downstream completed`, còn solution phải có `Request canceled` và không có `Downstream completed`.

## 6. Alternative fixes

- Với real HTTP calls, truyền token vào `HttpClient.SendAsync` / `GetAsync` overload phù hợp.
- Với EF Core, truyền token vào `ToListAsync`, `SaveChangesAsync` và các async query APIs.
- Tạo linked token khi cần kết hợp request cancellation với service-level timeout.

## 7. Wrong or misleading fixes

- Chỉ kiểm tra `IsCancellationRequested` ở đầu method: cancellation có thể xảy ra sau check.
- Nuốt `OperationCanceledException` rồi trả success: làm telemetry và semantics sai.
- Dùng `Task.Run` để "cancel nhanh hơn": không giải quyết token propagation cho I/O-bound work.
- Chỉ cấu hình global timeout rất ngắn: timeout và caller cancellation là hai policy khác nhau.

## 8. Production implications

Orphaned work làm lãng phí connection pool, downstream quota và concurrency budget. Khi traffic lớn, abandoned requests có thể tiếp tục tạo tải sau khi client đã rời đi.

## 9. Trade-offs

Không phải operation nào cũng nên bị hủy ngay. Sau một irreversible side effect hoặc critical transaction boundary, cần quyết định rõ phần nào được phép cancel và phần nào phải hoàn tất để giữ consistency.

## 10. What a Senior engineer should notice

Cancellation là một contract xuyên layer. API signatures, timeout policy, transaction boundary, logging và exception mapping phải thống nhất semantics thay vì chỉ thêm `CancellationToken` cho có.
