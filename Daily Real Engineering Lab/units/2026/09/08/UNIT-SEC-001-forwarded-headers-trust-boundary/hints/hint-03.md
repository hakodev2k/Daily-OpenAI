# Hint 3

ASP.NET Core có forwarded-headers middleware với cấu hình trusted proxies/networks. Một thiết kế an toàn thường để framework xử lý forwarding semantics rồi business authorization đọc identity đã được chuẩn hóa, thay vì tự parse header ở endpoint/helper.
