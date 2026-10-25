# Hint 03 — Hợp đồng nhất quán

Xem xét một trạng thái suppression có version/epoch đơn điệu và cơ chế kiểm tra quyền hiển thị tại các public read boundaries. Thiết kế recovery để snapshot/reindex cũ không thể hồi sinh bản ghi; phân biệt xử lý link đã phát hành với việc ngăn phát hành link mới. Xác định lúc nào được phép ACK.
