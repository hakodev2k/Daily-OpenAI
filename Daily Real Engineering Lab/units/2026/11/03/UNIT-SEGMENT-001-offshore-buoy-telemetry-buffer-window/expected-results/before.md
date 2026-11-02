# Before
Starter gốc: `./reproduce.ps1` exit 0; `full-buffer` và `empty-window` PASS, `offset-two` và `trailing-window` FAIL. `./verify.ps1` exit 1. Header có độ dài hợp lệ nhưng payload byte sai. Nếu không tái hiện, kiểm tra `dotnet --version`, mã starter có còn nguyên không, rồi chạy `./run.ps1` để xem hex.
