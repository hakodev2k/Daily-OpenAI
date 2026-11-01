# Hint 03 — Semantics của rethrow

Trong C#, phân biệt `throw;` với `throw error;` khi đang ở trong cùng một `catch`. Nếu phải truyền exception sang ngữ cảnh khác, nghiên cứu `ExceptionDispatchInfo` và lý do không thể dùng `throw;` bên ngoài `catch`.
