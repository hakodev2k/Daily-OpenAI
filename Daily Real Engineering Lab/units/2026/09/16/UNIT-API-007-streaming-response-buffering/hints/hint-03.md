# Hint 3

`IAsyncEnumerable<T>` chỉ mang lại streaming khi các layer trung gian giữ được tính incremental của sequence. Xem liệu có thể chuyển tiếp từng item ngay khi upstream yield hay không.