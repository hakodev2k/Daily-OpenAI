# Hint 3

Kiểm tra cách `TaskCompletionSource` chạy continuations khi `SetResult` được gọi. Tìm cách tách continuation execution khỏi call stack đang giữ synchronization primitive, hoặc thay đổi boundary để signal chỉ được phát sau khi rời critical section.