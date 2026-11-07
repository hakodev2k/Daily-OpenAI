# Hint 3 — Chính sách trust
Khảo sát `HttpClientHandler.AllowAutoRedirect` và cách `Authorization` được xử lý. Nếu tự theo redirect, chỉ gửi credential tới origin đã được xác thực, giới hạn số hop, và tạo request mới cho mỗi hop.
