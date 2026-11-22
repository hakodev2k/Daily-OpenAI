# Senior extension
Thiết kế endpoint duyệt nhiều holds đồng thời. Đặt `rowversion` hoặc concurrency token trong điều kiện update; kiểm tra số rows affected để trả 409 khi stale. Chứng minh rằng đọc response sau write không vô tình vượt qua transaction boundary; bổ sung test cho 0 row và concurrent approval. So sánh chi phí reload từng entity với projection một query.
