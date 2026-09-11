# Hint 3

Azure Blob Storage hỗ trợ optimistic concurrency bằng `ETag` và conditional request (`If-Match`). Write chỉ nên được chấp nhận khi version mà editor đã đọc vẫn là version hiện tại.
