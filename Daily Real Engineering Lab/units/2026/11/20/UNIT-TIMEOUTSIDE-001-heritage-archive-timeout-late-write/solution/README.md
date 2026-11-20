# Reference solution — chỉ xem sau khi thử

`Task.WhenAny` chỉ ngừng chờ, không hủy `SaveAsync`; task nền tiếp tục chạy và có thể ghi sau TIMEOUT. So sánh với [Program.cs](Program.cs): truyền `CancellationToken` vào thao tác trì hoãn, kiểm tra token trước side effect và chỉ trả TIMEOUT sau khi operation đã kết thúc bằng cancellation. Trong hệ thống thực tế, phải truyền cancellation đến DB/HTTP khi được hỗ trợ; cancellation không thể rollback một commit đã xảy ra. Phải xác định rõ commit boundary và dùng unique request ID/idempotency key cho retry.

## Wrong fixes
- Chỉ tăng timeout: che triệu chứng, không giải quyết late side effect khi timeout vẫn xảy ra.
- `Task.Run` hoặc bỏ qua task lỗi: không tạo cancellation.
- Chỉ hủy `Task.Delay(timeoutMs)`: không tác động đến tác vụ ghi.
- Kiểm tra token trước một thao tác commit không hủy được: vẫn có race; cần transaction/idempotency ở storage.

## Verification
Sao chép logic reference vào `starter/Program.cs` sau khi tự làm; chạy `./verify.ps1`. Kỳ vọng TIMEOUT slow, AT_RESPONSE 0, AFTER_WAIT 0, OK fast, FINAL 1.
