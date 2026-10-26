# Concepts after lab — SPOILER

- `PeriodicTimer.WaitForNextTickAsync` cung cấp tín hiệu chu kỳ cho một consumer, không hứa replay từng tick bị bỏ lỡ.
- Business window là khoảng được xác định từ checkpoint + interval; nó tồn tại độc lập với số wake-up.
- Checkpoint sau side effect thành công bảo đảm không **skip** trong mô hình tuần tự, nhưng có thể **replay** khi crash giữa side effect và checkpoint; cần idempotency.
- `now` được chụp một lần mỗi wake-up để giới hạn vòng đối soát; trong production cần giới hạn batch và phân tách lease/ownership nếu có nhiều worker.
- Quan sát nên đo backlog/lag theo giờ nghiệp vụ thay vì chỉ đo timer tick hoặc worker heartbeat.