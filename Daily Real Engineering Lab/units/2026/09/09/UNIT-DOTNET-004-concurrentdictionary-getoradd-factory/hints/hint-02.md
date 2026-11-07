# Hint 02

Xem contract của overload `GetOrAdd(key, valueFactory)` dưới contention.

Tự hỏi: nếu callback chạy ngoài internal lock để tránh giữ lock trong user code, điều gì có thể xảy ra khi hai thread cùng miss một key?
