# Hint 2 — Quan sát pipeline
Kiểm tra `HttpContext.Request.Scheme` mà mỗi middleware nhìn thấy. Middleware nào có thể kết thúc request trước khi endpoint được chạy? So sánh thứ tự các bước xử lý thông tin từ proxy với bước quyết định redirect.
