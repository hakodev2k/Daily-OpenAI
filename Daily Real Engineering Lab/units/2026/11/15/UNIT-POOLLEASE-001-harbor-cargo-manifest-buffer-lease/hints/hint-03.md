# Hint 3
Kiểm tra hợp đồng `ArrayPool<T>.Return`: một `ReadOnlyMemory<T>` không sở hữu buffer. Xem xét copy sang vùng nhớ thuộc caller hoặc một lease có Dispose.
