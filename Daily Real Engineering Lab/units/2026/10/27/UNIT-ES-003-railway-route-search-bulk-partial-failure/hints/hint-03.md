# Hint 3 — Khái niệm
Đọc `items[].index.status` và `_id`; phân loại 2xx, lỗi có thể retry (ví dụ 429/5xx) và lỗi dữ liệu cần cách ly (ví dụ 400). Giới hạn retry và chỉ gửi lại item cần retry. Không sử dụng reference trước khi tự thử.
