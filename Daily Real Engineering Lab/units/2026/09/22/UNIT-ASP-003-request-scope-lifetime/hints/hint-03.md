# Hint 3

Một service giữ request-specific state không nên sống lâu hơn request scope. Kiểm tra registration của consumer đang giữ `RequestContext`, sau đó bật scope validation để xem container có thể giúp phát hiện cấu hình lifetime sai sớm hơn không.