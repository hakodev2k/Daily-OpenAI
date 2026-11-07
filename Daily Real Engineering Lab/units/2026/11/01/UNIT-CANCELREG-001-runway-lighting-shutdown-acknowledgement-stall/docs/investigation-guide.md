# Investigation guide

## Tình huống
Dịch vụ điều phối bảo trì sân bay cần xác nhận đã nhận yêu cầu shutdown, sau đó mới chờ flush telemetry. Gate mô phỏng downstream cố tình đóng cho đến khi test mở. Không có độ trễ mạng ngẫu nhiên.

## Thu thập evidence
1. Chạy `./run.ps1` và `./reproduce.ps1`, ghi giá trị acknowledgement và flush count.
2. Đặt breakpoint ở lời gọi yêu cầu hủy, callback bắt đầu, và sau khi gate mở.
3. Ghi timeline theo thread/call stack. Phân biệt `Task` hoàn thành với callback đã được gọi.
4. Kiểm tra `./verify.ps1` trên starter nguyên bản, ghi exit code.

## Giả thuyết cạnh tranh
- Downstream hoàn thành sai thứ tự: kiểm tra flush count khi gate đóng.
- Scheduler thiếu thread: xác định callback đã vào hay chưa.
- Caller đang đợi một thao tác khác: đối chiếu stack trace và thời điểm lời gọi trả về.
- Flush bị gọi lặp: gọi yêu cầu dừng lần hai và kiểm tra số đếm.

## Acceptance contract
Tín hiệu dừng trả về khi gate vẫn đóng; cleanup chỉ hoàn thành sau khi mở gate; cleanup đúng một lần; token đã cancelled trước khi đăng ký vẫn được xử lý; caller có Task để await lỗi và kết quả cleanup.
