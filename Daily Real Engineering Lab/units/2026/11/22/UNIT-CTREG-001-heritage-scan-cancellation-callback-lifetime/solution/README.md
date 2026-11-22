# Reference Solution — UNIT-CTREG-001

## Root cause
`CancellationToken.Register` trả về `CancellationTokenRegistration`. Starter bỏ qua handle này, vì vậy callback tiếp tục đăng ký với shutdown token ngay cả khi scan đã hoàn thành. Khi host hủy token sau ba scan thành công, cả ba callback vẫn chạy và làm sai audit.

## Sửa đúng
Giữ handle trong phạm vi xử lý bằng `using var registration = token.Register(...)`. `Dispose()` hủy đăng ký khi thoát phương thức; nếu cancellation xảy ra trong khi xử lý, callback vẫn được gọi một lần. Không dispose `CancellationTokenSource` của host trong mỗi scan vì quyền sở hữu thuộc host.

## Verify
`./scripts/verify.ps1` chạy trên `starter` (mã do người học sửa). So sánh với `solution/Program.cs` sau khi hoàn thành điều tra.

## Wrong fixes
- Chỉ xóa log lúc shutdown: che triệu chứng, callback vẫn sống và có thể kích hoạt side effect khác.
- Gọi `CancellationTokenSource.CancelAfter` với thời gian dài hơn: đổi thời điểm, không giải quyết lifetime.
- Tạo token mới cho mỗi scan mà không xác định ownership: phá shutdown propagation, tăng tài nguyên.
- Chỉ kiểm tra `IsCancellationRequested` lúc đăng ký: không ngăn callback được gọi về sau.

## Trade-offs
Registration disposal có thể chờ callback đang chạy; callback nên ngắn, thread-safe và không chặn lâu. Trong production cần lưu trạng thái scan bằng state machine/idempotent transition thay vì để audit callback tự ghi đè biên nhận thành công.
