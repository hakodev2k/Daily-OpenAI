# Incident log (synthetic)
- 06:00 — Chỉ màn hình điều phối: nhận đủ ba heartbeat ID.
- 06:03 — Kết nối màn hình an toàn: observer thứ hai fault với `InvalidOperationException`.
- 06:04 — CPU và memory không tăng đáng kể; chưa có bằng chứng lỗi mạng.
- 06:07 — Hủy một phiên theo dõi: phiên còn lại phải tiếp tục.
- Chạy `./reproduce.ps1` để tạo bằng chứng trên máy local, không cần thiết bị thật.

