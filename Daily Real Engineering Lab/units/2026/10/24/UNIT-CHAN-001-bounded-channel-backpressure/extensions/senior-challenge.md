# Senior Challenge — failure và shutdown
Không làm trước khi verify cơ bản.

1. Thêm CancellationToken cho producer bị chờ khi buffer đầy: đảm bảo cancellation không được tính là accepted.
2. Mô phỏng consumer throw ở sự kiện 23; thiết kế propagation và không để producer chờ vô hạn sau failure.
3. Thiết kế chính sách shutdown 10 giây: khi nào dừng nhận, khi nào drain, khi nào ghi durable handoff?
4. Phân tích vì sao in-memory queue không thể tự bảo đảm exactly-once khi process crash và khi scale nhiều replica.
