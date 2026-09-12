# Reference Solution

> Chỉ xem sau khi bạn đã reproduce và tự thử sửa.

## Symptoms
Publisher log `Publish completed`, rồi subscriber mới throw và process có thể bị terminate.

## Evidence
Subscriber được gắn vào `Action<string>` nhưng implementation là `async void`. Sau điểm `await`, caller không có `Task` để await hay inspect.

## Root cause
`async void` không cung cấp Task-based completion/failure contract cho publisher. Exception sau asynchronous continuation không quay trở lại synchronous `try/catch` bao quanh `Publish()`.

## Why the fix works
Thay subscriber contract bằng `Func<string, Task>` và để `PublishAsync` return/await toàn bộ Task. Publisher lúc này sở hữu lifecycle của publish operation và có thể quan sát failure.

## How to verify
Chạy `./verify.ps1`. Script kiểm tra chính `starter/`: exit code phải bằng 0, có `Publisher caught`, có `Process finished normally`, và không được log `Publish completed` cho operation thất bại.

## Alternative fixes
- Một event abstraction hỗ trợ async handlers và policy rõ ràng cho sequential/parallel execution.
- Queue/background worker nếu notification không thuộc request/operation transaction boundary; khi đó cần retry, idempotency và observability riêng.

## Wrong / tempting fixes
- Bọc riêng lời gọi `Publish()` bằng thêm `try/catch`: vẫn không sở hữu continuation của `async void`.
- Thêm `Task.Delay` để chờ: timing không tạo ra exception contract.
- Nuốt exception bên trong subscriber: có thể tránh crash nhưng che failure nếu business yêu cầu caller biết kết quả.
- `Task.Run` quanh publish: chỉ di chuyển execution context, không biến `async void` thành Task có thể await.

## Production implications
Chọn rõ semantics: subscriber failure có fail publish không, handlers chạy tuần tự hay song song, timeout/cancellation ra sao, và side effect có cần durable messaging hay không.

## Trade-offs
`Task.WhenAll` đơn giản và giữ failure observable, nhưng chạy handlers song song. Nếu ordering hoặc dependency giữa handlers quan trọng, await tuần tự có thể phù hợp hơn.

## Senior engineer should notice
Async API design là ownership contract. Nếu caller cần biết completion/failure, API phải trả về một awaitable operation thay vì fire-and-forget ngầm định.
