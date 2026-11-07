# Reference Solution — SPOILER

Chỉ đọc sau khi reproduce và thử fix trong `starter/`.

## 1. Symptoms
Cả hai audit receipt có đúng event ID nhưng correlation ID bị `missing`. Đường async kiểm soát vẫn có ID.

## 2. Evidence
`Task.Run` trả `control` đúng cho mỗi request, trong khi callback ghi audit trả `missing` ở cả CASE-A và CASE-B. Không có network, SQL hay timing-dependent mutation.

## 3. Root cause
`AsyncLocal<T>` là ambient data gắn với `ExecutionContext`. `Task.Run` flow ngữ cảnh thực thi của caller, còn `ThreadPool.UnsafeQueueUserWorkItem` cố ý **không flow** `ExecutionContext`. Callback đọc `RequestContext.CorrelationId.Value` từ worker nên không có correlation ID của request đã dispatch. Đây không phải lỗi của `TaskCompletionSource`.

## 4. Why the fix works
Reference code lấy `RequestContext.CorrelationId.Value` tại thời điểm dispatch và đặt vào `WorkState.CorrelationAtDispatch`. Callback dùng dữ liệu bất biến của job thay vì ambient context. Hai request đồng thời giữ riêng ID; cách này vẫn áp dụng khi thay ThreadPool bằng queue, message broker hoặc process khác.

## 5. How to verify
Chạy `./verify.ps1` **trên starter đã sửa**. Reference tự kiểm tra: `dotnet run --project ./solution/CorrelationLab.csproj -- --verify`. Cả hai ID phải đúng và không bị lẫn. `reproduce.ps1` chỉ dành cho trạng thái lỗi ban đầu.

## 6. Alternative fixes
Có thể dùng `ThreadPool.QueueUserWorkItem` khi thực sự muốn flow `ExecutionContext` và hiểu chi phí/phạm vi context. Với audit có thể durable hoặc remote, truyền explicit correlation/trace identifiers trong message envelope thường an toàn hơn. `Activity.Current` và W3C trace context cũng cần propagation qua boundary phù hợp, không tự xuất hiện ở worker không flow context.

## 7. Wrong / tempting fixes
- Thêm `Task.Delay` hay lock: không làm ambient context tự truyền qua boundary.
- Đổi sang `Task.Run` chỉ để test pass: có thể che vấn đề ownership và làm phụ thuộc scheduler; hợp lệ trong trường hợp khác nhưng không giải quyết hợp đồng message bền vững.
- Đặt một biến `static string` dùng chung: hai request đồng thời có thể ghi đè, gây cross-request contamination.
- Dùng `ExecutionContext.Capture/Run` cho mọi background job: có thể giữ nhiều ambient state không mong muốn, kể cả dữ liệu request nhạy cảm.

## 8. Production implications
Mỗi audit envelope nên có correlation ID và tenant/trace context cần thiết một cách tường minh, validate trước khi enqueue, không log secret. Nếu công việc phải bền vững, cần durable queue, retry, idempotency và telemetry riêng; simulator này chưa mô phỏng các thuộc tính đó.

## 9. Trade-offs
Explicit propagation thêm trường và trách nhiệm validate nhưng giảm phụ thuộc vào host/scheduler và làm contract có thể serialize. Flow toàn bộ `ExecutionContext` thuận tiện trong cùng process, nhưng kém rõ ràng và không thay thế propagation qua process boundary.

## 10. What a Senior engineer should notice
Phân biệt data flow với control flow; `TaskCompletionSource` bảo đảm completion chứ không mang request context; không dựa vào timing để chứng minh correctness. Kiểm tra trường hợp đồng thời và failure path, xác định contract khi thiếu ID, đánh giá rủi ro dữ liệu ambient bị truyền ngoài ý muốn.
