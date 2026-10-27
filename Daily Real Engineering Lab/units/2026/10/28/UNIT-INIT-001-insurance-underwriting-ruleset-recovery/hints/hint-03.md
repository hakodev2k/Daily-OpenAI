# Hint 03 — Cơ chế cụ thể

Lazy<Task<T>> giữ lại Task đã tạo, kể cả khi Task đó faulted. Tách khái niệm "chỉ một lần tải đang chạy" khỏi "cache mãi một kết quả thành công"; chỉ tái tạo tác vụ khi chưa có tác vụ hoặc tác vụ trước đã faulted/canceled.
