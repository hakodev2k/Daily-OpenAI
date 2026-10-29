# Bài toán nghiệp vụ và hợp đồng

Hệ thống nhận `WindowRequest(Id, LocalStart, UtcOffsetMinutes)`. `LocalStart` là wall-clock time tại `America/New_York`, không phải UTC; `UtcOffsetMinutes` (nếu có) do đối tác khai báo, tính bằng phút so với UTC. Ví dụ `-240` là UTC-04:00.

**Acceptance contract cần bảo toàn:**
- Thời gian địa phương hợp lệ và không có nhiều cách hiểu: chấp nhận khi offset vắng mặt hoặc đúng với timezone; chuyển sang UTC.
- Khi cùng local time tương ứng với nhiều UTC instants: chỉ chấp nhận nếu đối tác cung cấp offset hợp lệ để phân biệt; nếu không, từ chối có giải thích.
- Khi local time không tồn tại tại timezone: từ chối, không tự động dịch giờ.
- Offset không phù hợp với timezone tại thời điểm đã khai báo: từ chối.
- Giữ nguyên `Id` và không phát sinh side effect trong hàm chuyển đổi.

Lưu ý: nghiệp vụ yêu cầu **không tự chọn một ca** khi thông tin không đủ. API ngoài phạm vi lab phải biến lỗi input thành phản hồi validation phù hợp, có telemetry và tài liệu migration.

Tập trung vào sửa `starter/WindowScheduler.cs`. Các file `tests/` là acceptance contract độc lập và không nên sửa để làm bài pass.
