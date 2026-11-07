# Reference solution — chỉ đọc sau khi tự thử

## Root cause
`WaitAsync(cancellationToken)` có thể ném trước khi lấy được permit. `finally` vẫn gọi `Release()`, tạo permit giả. Trong fixture semaphore không khai báo maxCount nên permit giả xuất hiện rõ qua CurrentCount; production cần cấu hình giới hạn và xử lý exception phù hợp.

## Reference implementation
Xem [Program.cs](Program.cs). Thay đổi quan trọng: `await WaitAsync` ở ngoài `try/finally`; chỉ bắt đầu cleanup sau acquire thành công.

## Wrong fixes
- Bắt và bỏ qua mọi exception: che giấu lỗi mà không bảo vệ ownership.
- Tăng `maxCount`: làm sai hợp đồng giới hạn đồng thời.
- Tắt cancellation: tránh triệu chứng bằng cách làm hỏng API contract.

## Trade-offs
SemaphoreSlim không có lease token; ownership phải được cấu trúc bằng control flow hoặc một scope riêng. Nếu delegate không hỗ trợ cancellation, cancellation trong khi đang gửi phải được xử lý riêng theo business semantics.
