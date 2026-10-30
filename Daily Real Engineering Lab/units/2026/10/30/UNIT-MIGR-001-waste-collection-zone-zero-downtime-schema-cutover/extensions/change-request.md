# Change request — mở sau khi hoàn thành thiết kế đầu tiên

Ngày thứ 10: vùng Z-01 và Z-03 sẽ hợp nhất thành Z-09 từ **2026-12-01**. Báo cáo tuyến lịch sử phải giữ vùng có hiệu lực tại **thời điểm điều phối**; tuyến tương lai dùng vùng mới. Không được cập nhật ngược lịch sử.

Cập nhật Decision Record: cần dữ liệu hiệu lực theo thời gian hay snapshot bất biến nào? Test một tuyến 2026-11-30 và một tuyến 2026-12-01. Phân biệt `updated_at` của record với event time của điều phối.
