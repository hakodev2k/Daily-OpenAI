# Reference Solution — inspect only after reproducing and attempting your own fix

## 1. Symptoms

Client cancellation xảy ra nhưng export work vẫn tiếp tục chạy gần đủ thời gian downstream operation. Trong burst, các abandoned request tiếp tục giữ bounded concurrency slots và làm request mới bị queue dù CPU thấp.

## 2. Evidence

Reproduction cho thấy cả hai request token đều đã canceled trong khi `ActiveOperations` vẫn là `2` và `AvailableSlots` vẫn là `0`. Các slot chỉ trở lại sau khi `ReportStore.BuildAsync` tự kết thúc.

## 3. Root cause

`ExportService` nhận cancellation signal từ request, nhưng `OperationBudget` tạo một timeout token riêng và chỉ expose token đó. Vì vậy request-abort signal không còn nằm trong cancellation contract được truyền vào `SemaphoreSlim.WaitAsync` và downstream work.

Trong ASP.NET Core thực tế, signal tương ứng là `HttpContext.RequestAborted`. Internal timeout và request lifetime là hai cancellation sources độc lập; thay một source bằng source kia làm mất semantics của source còn lại.

## 4. Why the fix works

Tạo linked token source từ request token và internal timeout token. Linked token chuyển sang canceled khi **bất kỳ** source nào cancel. Cùng token đó được truyền qua gate và downstream call, nên cancellation semantics nhất quán xuyên suốt operation.

Thay implementation của `OperationBudget` bằng nội dung trong [`OperationBudget.fixed.cs`](OperationBudget.fixed.cs).

## 5. How to verify

Sau khi áp dụng fix vào `starter/Program.cs`:

```powershell
./verify.ps1
```

Verify kiểm tra learner-editable code path và yêu cầu:

- abandoned work giải phóng slots trong bounded interval
- `ActiveOperations` trở về `0`
- `AvailableSlots` trở về `2`
- healthy export vẫn hoàn thành bình thường

## 6. Alternative fixes

Có thể gọi `CancellationTokenSource.CreateLinkedTokenSource(requestAborted)` rồi dùng `CancelAfter(timeout)`. Cách này cũng hợp lệ nếu ownership/disposal rõ ràng. Có thể encapsulate policy này thành reusable operation-budget abstraction nếu nhiều endpoints có cùng rule.

## 7. Wrong / tempting fixes

- **Tăng semaphore capacity:** chỉ kéo dài thời gian trước khi pressure xuất hiện và tăng downstream concurrency.
- **Tăng server timeout:** làm abandoned work sống lâu hơn.
- **Disable client cancellation:** che signal thay vì propagate nó.
- **Bắt `OperationCanceledException` rồi bỏ qua trước khi work dừng:** không giải phóng resource nếu underlying operation vẫn chạy.
- **Chỉ kiểm tra `requestAborted.IsCancellationRequested` một lần trước downstream call:** cancellation có thể xảy ra sau check; cooperative cancellation phải tiếp tục được observe.

## 8. Production implications

Cancellation propagation là capacity-management mechanism, không chỉ là UX detail. Khi request lifetime bị cắt ở reverse proxy, mobile disconnect hoặc browser navigation, continuing work có thể tiêu thụ DB connections, HTTP sockets, semaphore permits, CPU hoặc third-party quota mà không còn client nhận kết quả.

## 9. Trade-offs

Không phải mọi operation đều nên dừng khi HTTP client rời đi. Ví dụ một committed payment workflow hoặc durable background command có thể phải hoàn thành độc lập với request lifetime. Khi đó boundary đúng là persist/queue durable work rồi tách lifecycle khỏi `HttpContext`, không phải silently ignore cancellation trong request-scoped flow.

## 10. What a Senior engineer should notice

Senior engineer cần phân biệt ít nhất ba lifetimes: client/request lifetime, service-level timeout budget và durable business operation lifetime. Việc chọn token nào được propagate phải phản ánh ownership và business semantics, không chỉ API convenience.

## References

Xem [`docs/references.md`](../docs/references.md).
