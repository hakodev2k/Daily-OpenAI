# Hint 03 — Cơ chế cần kiểm tra
HttpResponseMessage.Dispose() giải phóng HttpContent; StreamContent quản lý stream bên dưới. Khi API trả stream cho caller, vòng đời của HTTP response phải được giữ cho đến lúc caller đóng stream, hoặc toàn bộ việc đọc phải hoàn tất trước khi đóng response. Không thay streaming bằng đọc toàn bộ CSV vào RAM.
