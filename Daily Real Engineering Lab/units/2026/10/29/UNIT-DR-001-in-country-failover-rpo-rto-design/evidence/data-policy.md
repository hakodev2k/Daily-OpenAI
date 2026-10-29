# Data residency policy — synthetic

- PII bao gồm tên người giữ thẻ, định danh, lịch sử vào cửa gắn với cá nhân và audit có thể tái định danh.
- Primary, replica, snapshots, backups, dead-letter payload và crash dump chứa PII phải nằm trong X.
- Region Y nằm ngoài X. Encryption at rest và private networking không tự tạo ngoại lệ.
- Site X2 cách X1 80 km, nằm trong X, được phép lưu PII nếu đạt kiểm toán bảo mật.
- Telemetry đã xác minh không chứa PII có thể ra khỏi X; không được mặc định mọi log là phi PII.
- Ngoại lệ chỉ do Legal/Compliance phê duyệt, không phải quyết định đơn phương của Engineering.

Đây là chính sách giả lập, không phải tư vấn pháp lý.
