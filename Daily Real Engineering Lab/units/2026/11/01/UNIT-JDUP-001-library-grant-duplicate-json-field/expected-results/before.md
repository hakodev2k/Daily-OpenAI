# Trước khi sửa

- `./scripts/reproduce.ps1` trả exit code 0 trên starter gốc.
- Console hiển thị `accepted=True; persisted=42000` dù ngưỡng tự động là 5000.
- `./scripts/verify.ps1` thất bại: contract runner phát hiện dữ liệu không rõ ràng, trường không hợp lệ hoặc lỗi parse không được xử lý nhất quán.
- Không có tiền thật, database hay API bên ngoài bị tác động.
