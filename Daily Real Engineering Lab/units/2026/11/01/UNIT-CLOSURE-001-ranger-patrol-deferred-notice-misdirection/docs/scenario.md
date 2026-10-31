# Tình huống nghiệp vụ

Mỗi ca tuần tra phát sinh một thông báo bàn giao với `PatrolId`, `RecipientId` và `Text`. Tại thời điểm xếp batch, hệ thống chỉ được tạo công việc chờ xử lý; không được gửi ngay. Khi worker chạy, mỗi thông báo phải được chuyển đến đúng người nhận và giữ nguyên thứ tự batch.

Giới hạn: không dùng gateway thật; chỉ dùng `FakeGateway` và `DeferredQueue` để tái hiện đúng ranh giới giữa enqueue và execute. Kiểm tra cả **số lượng** và **định danh** thay vì chỉ kiểm tra tổng số thông báo.

Mở rộng: nếu worker chạy song song, chính sách thứ tự cần được định nghĩa lại; nếu gửi tới provider thật, cần thêm idempotency key, retry classification và audit trail. Đây là phần thảo luận, không phải điều kiện PASS của lab cơ bản.
