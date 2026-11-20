# Hợp đồng và giả định
- Trạng thái: `Pending -> Approved -> Released`; chỉ `Released` mới phát sự kiện.
- `BloodUnitId` là định danh duy nhất; mỗi lần thay đổi trạng thái có `Version` tăng đơn điệu.
- Một sự kiện phải có `EventId`, `BloodUnitId`, `Version`, `OccurredAtUtc`, `SchemaVersion`.
- Broker có thể trả timeout dù đã nhận message; consumer có thể crash sau khi tạo shipment nhưng trước ACK.
- Consumer chỉ được tạo tối đa một shipment cho cặp `BloodUnitId + Version`; cùng `EventId` gửi lại phải vô hại.
- Không có transaction xuyên SQL Server và broker; không giả định FIFO toàn cục.
- Giả định mục tiêu: 99.9% sự kiện được giao trong 60 giây lúc hệ thống khỏe, RPO nghiệp vụ 0 đối với trạng thái đã commit; khi broker down, cho phép backlog tăng và cảnh báo.
- Cần giải thích cách xử lý schema evolution, poison messages, retention, quyền truy cập và replay sau 24 giờ outage.
- Không cho phép giải pháp dựa trên polling API trạng thái hiện tại thay thế toàn bộ lịch sử sự kiện nếu consumer cần transition events.
