# Incident evidence — 2026-10-30 (synthetic)

Bối cảnh: kho nhận bảng kê hoàn trả thiết bị thể thao; file được báo rejected nhưng nhân viên thấy số lượng đã đổi. Đây là dữ liệu mô phỏng cố định trong source, không phải log production.

| Tín hiệu | Giá trị quan sát kỳ vọng ở starter |
|---|---|
| Batch status | Rejected |
| Exception family | System.Text.Json.JsonException |
| Last record | GEAR-103 |
| CommittedCount after rejection | 2 |
| ObservedIds | GEAR-101, GEAR-102 |
| Valid batch | 3 record hợp lệ, có thể import |

Hypotheses cần kiểm chứng: (1) parsing chỉ lỗi sau khi đã thấy record đầu; (2) store bị race; (3) validation nghiệp vụ thiếu; (4) việc publish xảy ra ở ranh giới không phù hợp.

Không có concurrency, network hay external database trong reproduction này; đừng quy mọi sai lệch cho race condition. Thu thập output thực tế từ reproduce.ps1 trước khi kết luận.
