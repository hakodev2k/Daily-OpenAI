# After — starter do người học sửa
Với cùng burst 80 sự kiện:
- `AllPublishedBeforeRelease=False`: producer chưa thể đẩy hết khi downstream đứng lại.
- `AcceptedBeforeRelease<=18`, `PeakOutstanding<=18` (mức giới hạn của bài kiểm tra tính cả phần đang xử lý và sai khác nhỏ do cập nhật telemetry).
- Sau khi mở gate, tất cả 80 sự kiện được chấp nhận và xử lý đúng một lần trong harness.
- `./verify.ps1` exit code 0 và in `VERIFIED`.

Không dùng thời gian mili-giây cố định làm tiêu chí pass/fail. Trong production cần đo thêm p95 thời gian chờ producer và lượng backlog.
