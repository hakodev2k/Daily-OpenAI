# Hint 3

Thiết kế lại subscriber contract để mỗi handler trả `Task`, rồi để publisher trả/await operation tổng hợp thay vì dùng `Action<T>` cho asynchronous work.
