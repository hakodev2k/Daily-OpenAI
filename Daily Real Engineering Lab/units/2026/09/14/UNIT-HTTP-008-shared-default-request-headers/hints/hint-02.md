# Hint 2

`HttpClient` được thiết kế để reuse. Điều đó không có nghĩa mọi mutable property trên client đều phù hợp để chứa dữ liệu thay đổi theo từng tenant/request. Kiểm tra thời điểm authorization được đọc để tạo request thực tế.
