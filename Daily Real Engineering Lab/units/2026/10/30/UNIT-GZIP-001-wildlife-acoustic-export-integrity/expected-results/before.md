# Before — Starter chưa sửa

- `reproduce.ps1` trả exit code 0 khi phát hiện artifact có header GZIP nhưng thiếu/không hợp lệ footer.
- `verify.ps1` trả exit code khác 0 vì ít nhất một trường hợp không đạt đầy đủ header, footer, roundtrip.
- `roundtrip` có thể true hoặc false tùy cách decoder xử lý file chưa hoàn chỉnh; không dùng nó làm điều kiện duy nhất.
- Ghi lại kết quả thực tế; không phụ thuộc kích thước byte chính xác.
