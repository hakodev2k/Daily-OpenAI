# Sau khi sửa
`./verify.ps1` trả 0, `Accepted: 2` và `E-001, E-003`. Hai biểu diễn của cùng một mã vé được tính một lần; mã khác case vẫn riêng; EventId của lần quét đầu tiên và payload gốc không bị thay đổi.

Không cần benchmark: tiêu chí là tính đúng và ổn định, không phải độ trễ.
