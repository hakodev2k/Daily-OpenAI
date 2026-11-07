# Scenario — Regional GIS tile export
Service xử lý ảnh địa hình lớn trên máy có RAM hạn chế. Một node chỉ được render tối đa một tile tại một thời điểm. A bắt đầu render và đang chờ dữ liệu, B đến sau và bị client hủy trước khi được xử lý, C đến sau nữa. Dịch vụ vẫn báo MaxWorkers=1 nhưng telemetry có lúc ghi activeRenderers=2.

Business invariant: **không bao giờ có hai renderer đang chạy cùng lúc** khi cấu hình capacity bằng một. Hủy một request chưa chạy phải không ảnh hưởng capacity dành cho request khác. Renderer có exception không được làm cổng bị kẹt vĩnh viễn.

Phạm vi: mô phỏng in-process, không giả lập queue phân tán, nhiều replica hay cancellation của hệ điều hành. Không dùng timing benchmark làm tiêu chí chính.
