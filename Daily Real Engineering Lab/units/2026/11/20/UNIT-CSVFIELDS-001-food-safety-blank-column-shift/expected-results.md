# Kết quả mong đợi

- Starter: `S-101` hợp lệ; `S-102` bị reject dù đủ bốn vị trí CSV. `reproduce.ps1` xác nhận triệu chứng bằng exit code 2.
- Sau sửa: `S-102` có `PesticidePpm=""`, `Status="PENDING"`; cả ba bản ghi hợp lệ đều map đúng.
- Dòng thừa trường `S-999,East,0.02,PASS,EXTRA` phải bị từ chối; không được lặng lẽ bỏ qua.
- `verify.ps1` phải chạy code trong `starter/`, không chạy solution.
- Nếu lỗi không reproduce, xác nhận chạy từ đúng unit và `dotnet --version` là SDK 8 hoặc SDK tương thích build net8.0.
