# Investigation guide

1. Chạy `scripts/reproduce.ps1`, lưu output nguyên bản vào workspace.
2. Phân loại tín hiệu: exception type, exception message, audit count, stack frame. Dòng log hạ tầng giả định “partner payload malformed” không chứng minh tầng nào làm mất frame.
3. Đặt giả thuyết: lỗi xuất phát ở parser nhưng stack bị thay đổi khi đi qua tầng điều phối; hoặc compiler tối ưu call chain; hoặc telemetry đã lọc frame.
4. Đọc code `starter/` và xác định điểm kiểm chứng từng giả thuyết. Đừng bỏ qua `MethodImplOptions.NoInlining` trong fixture.
5. Chỉ thay đổi phần cần thiết. Chạy `scripts/verify.ps1` và so sánh cả correctness lẫn observability.

Mục tiêu là sửa diagnostic contract, không thay đổi logic parse, không thêm retry và không nuốt exception.
