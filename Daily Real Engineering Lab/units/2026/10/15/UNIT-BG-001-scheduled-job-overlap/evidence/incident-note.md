# Incident note (dữ liệu giả lập, dùng để luyện điều tra)

- Một instance worker đang chạy; deployment replicas = 1.
- Downstream latency tăng từ khoảng 5ms lên 200–400ms.
- Scheduler interval được cấu hình 20ms trong harness.
- Không có retry policy hay message broker trong bài này.
- CPU không bão hòa; số `BATCH_START` tăng trong khi batch đầu chưa hoàn tất.
- Không coi ghi chú này là kết quả đo từ môi trường của bạn. Hãy lấy số liệu thực tế bằng `reproduce.ps1`.

**Nhiễu có chủ đích:** Một log warning về DNS cache không xuất hiện trong đường chạy của harness; không đủ evidence để kết luận nguyên nhân.
