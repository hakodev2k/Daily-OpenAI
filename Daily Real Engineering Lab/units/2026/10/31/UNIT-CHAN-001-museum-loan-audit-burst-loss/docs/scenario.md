# Scenario — Chain of custody

Bảo tàng quốc gia nhận lô hiện vật mượn từ nhiều cơ sở. Mỗi lần quét mã chuyển giao cần một audit event theo sequence. Đợt quét 9 event đến nhanh hơn tốc độ lưu trữ. Dashboard dùng số thao tác được admission; audit dùng số event thực sự persist.

Yêu cầu: không bỏ event đã xác nhận; giữ thứ tự trong một luồng quét; lỗi persistence phải được phản hồi. Callback persist mô phỏng boundary lưu trữ.

Investigation: (1) Counter nào là bằng chứng durability? (2) Khi backlog đầy, hợp đồng producer là gì? (3) Consumer cần khởi chạy lúc nào? (4) Sink lỗi trong lúc producer chờ thì sao?

Log tại evidence là **fixture tổng hợp**, không phải telemetry đo từ runtime.
