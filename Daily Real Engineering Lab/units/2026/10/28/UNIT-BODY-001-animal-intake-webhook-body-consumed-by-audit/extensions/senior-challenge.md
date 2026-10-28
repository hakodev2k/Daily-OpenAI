# Senior Challenge
Thiết kế chính sách audit cho webhook tối đa 8 MiB nhưng không được log dữ liệu nhạy cảm. Trình bày trade-off giữa:
1. Buffer toàn bộ body với ngưỡng memory/disk và 413.
2. Audit metadata + hash của body (không lưu raw payload).
3. Streaming tee có bounded storage.

Xác định error contract cho client disconnect, cancellation, vượt kích thước, JSON lỗi; đề xuất metrics, tests và chính sách xóa dữ liệu audit. Không dùng raw PII làm log labels.
