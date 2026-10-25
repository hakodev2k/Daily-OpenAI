# Hint 03 — gợi ý khái niệm

Hợp đồng nghiệp vụ nói quota thuộc về tenant, không thuộc về endpoint. So sánh khóa phân vùng hiện tại với một định danh tenant **đã xác thực**, và quyết định khóa fallback khi không có claim. Tránh đọc tenant ID từ header tùy ý do client tự cung cấp.
