# Sau khi sửa

- `verify.ps1` trả exit code 0, `Contract failures: 0`.
- Mỗi segment không rỗng, UTF-8 hợp lệ và không vượt byte budget.
- Không chia giữa grapheme cluster; ghép theo thứ tự phục hồi chính xác payload gốc.
- Payload rỗng trả danh sách rỗng; text element lớn hơn budget bị từ chối rõ ràng.
- Kiểm tra thêm: ký tự nhiều byte, combining marks, emoji, budget 0 và payload có surrogate lỗi.
