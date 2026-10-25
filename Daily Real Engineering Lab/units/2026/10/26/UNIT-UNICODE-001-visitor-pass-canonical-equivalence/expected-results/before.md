# Trước khi sửa
`./reproduce.ps1` trả 0 và báo `Accepted: 3` với `E-001, E-002, E-003`; `E-004` đã bị loại vì lặp chính xác. `./verify.ps1` trả 1.

Nếu khác kết quả: kiểm tra SDK 8, file starter chưa bị chỉnh sửa và console đang chạy đúng project `starter/BadgeImport.csproj`.
