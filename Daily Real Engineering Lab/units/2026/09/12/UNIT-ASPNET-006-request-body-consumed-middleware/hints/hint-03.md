# Hint 3

Tìm ASP.NET Core API hỗ trợ request-body buffering. Sau khi đọc, đừng quên trả read position về đầu trước khi gọi `next`, đồng thời tránh đóng stream do request sở hữu.
