# Hint 03

ASP.NET Core có `ForwardedHeadersMiddleware`. Cấu hình cần vừa xử lý `X-Forwarded-Proto`, vừa xác định proxy nào được tin cậy, và middleware phải chạy trước logic phụ thuộc scheme.