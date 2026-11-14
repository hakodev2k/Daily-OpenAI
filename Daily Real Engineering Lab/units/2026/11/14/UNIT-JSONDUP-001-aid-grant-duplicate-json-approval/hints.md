# Hints

1. So sánh **raw bytes** với các field mà hai consumer thực sự quan sát.
2. Kiểm tra hành vi của `EnumerateObject()` và `GetProperty()` khi có tên thuộc tính trùng nhau.
3. Xác định một chính sách thống nhất tại trust boundary: payload có tên thuộc tính trùng nhau có nên được chấp nhận không? Đừng chỉ sửa một consumer.
