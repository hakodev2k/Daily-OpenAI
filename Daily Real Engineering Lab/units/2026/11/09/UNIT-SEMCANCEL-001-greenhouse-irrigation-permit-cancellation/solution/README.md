# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms & evidence
Khi lệnh A đang giữ permit, lệnh B bị hủy lúc chờ; harness có thể thấy lệnh C đi vào trước khi A hoàn thành (`overlap=true`), và A có thể ném `SemaphoreFullException` lúc kết thúc. Lệnh B không được chạy action.

## Root cause
`WaitAsync(cancellationToken)` có thể throw trước khi acquire permit. Vì `WaitAsync` nằm trong `try`, `finally` vẫn gọi `Release` dù B chưa sở hữu permit. Permit count bị tăng sai, tạo overlap; release của chủ sở hữu hợp lệ có thể vượt `maxCount=1`.

## Fix
Xem [IrrigationController.cs](IrrigationController.cs). Chỉ bắt đầu `try/finally` **sau khi** `WaitAsync` hoàn tất thành công; lúc đó permit mới được sở hữu. `finally` bảo đảm release nếu action throw. Không nuốt `OperationCanceledException`.

## Verify
Sao chép nội dung reference vào `starter/IrrigationController.cs`, chạy `./verify.ps1`. Kết quả cần `canceled=True`, `firstCompleted=True`, `overlap=False`, `releaseFault=False`, `executed=0`. `./reproduce.ps1` sau sửa sẽ exit khác 0 vì lỗi không còn tái hiện.

## Alternatives
Có thể dùng biến `acquired` và release có điều kiện trong `finally`, nhưng thêm state dễ sai hơn. Nếu giới hạn concurrency thay đổi, cân nhắc `Channel<T>` hoặc queue có backpressure; không thay thế semaphore chỉ vì một bug cleanup.

## Wrong / tempting fixes
- Bắt rồi bỏ qua `SemaphoreFullException`: che lỗi count, không ngăn overlap.
- Bỏ cancellation: làm lệnh hủy tiếp tục chờ và phá yêu cầu vận hành.
- Tăng maxCount: vi phạm tính độc quyền của van.
- Dùng `lock` quanh `await`: không hợp lệ với C# lock và không giải quyết ownership async.

## Production implications / trade-offs
Log wait-start, acquired, released cùng operation ID; không log dữ liệu nhạy cảm. Đảm bảo action timeout, xử lý shutdown và không dùng semaphore in-memory để đồng bộ nhiều replica. Senior engineer phải phân biệt **attempted acquisition** với **successful ownership**, và test cancellation ở từng boundary.
