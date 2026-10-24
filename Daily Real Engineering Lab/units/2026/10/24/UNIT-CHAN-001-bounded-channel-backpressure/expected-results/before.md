# Before — starter gốc
Với 80 sự kiện, downstream bị giữ ở sự kiện đầu tiên:
- `AllPublishedBeforeRelease=True`.
- `AcceptedBeforeRelease=80/80`.
- `PeakOutstanding` xấp xỉ 80 (tùy thời điểm reader bắt đầu).
- Sau khi mở gate: `Accepted=80`, `Completed=80`, không mất dữ liệu.

Khi thay 80 bằng hàng trăm nghìn trong hệ thống thực, mức backlog có thể tăng theo burst. Bài lab cố ý chỉ dùng 80 để không gây áp lực bộ nhớ máy học.
