# Scenario — Dữ liệu bảo trì kiosk bike-share

Kiosk gửi UTF-8 bytes qua một đường truyền có thể chia thành nhiều đoạn bất kỳ. Backend nhận được đúng thứ tự byte, nhưng báo cáo có lúc xuất hiện ký tự thay thế. Hệ thống upstream khẳng định payload nguyên gốc hợp lệ và không có mất byte.

**Hợp đồng:** thông điệp UTF-8 hợp lệ phải được tái tạo giống hệt bất kể cách chia đoạn; byte sequence không hợp lệ phải bị từ chối thay vì lưu ghi chú đã biến dạng. Kiosk không gửi dữ liệu thật trong lab.

**Evidence:** harness in trạng thái correct, rejected, replacementChars và số chunk. Chứng cứ nhiễu có chủ ý: ASCII luôn qua được và có thể khiến đội vận hành kết luận nhầm rằng encoding đã đúng.

**Ràng buộc:** không được ghép payload bằng cách làm mất thứ tự, không được tự động xóa ký tự Unicode và không thay đổi tests.
