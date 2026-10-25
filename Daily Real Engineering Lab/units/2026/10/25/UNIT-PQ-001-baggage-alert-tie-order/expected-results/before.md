# Before — Starter
- ./scripts/reproduce.ps1 phải in REPRODUCED và trả exit code 0 khi chạy trên starter chưa sửa.
- Với input A,B,C,D có severity bằng nhau, expected là A,B,C,D; actual khác expected (trên .NET 8 với cấu hình của unit).
- ./scripts/verify.ps1 phải in ít nhất một FAIL và trả exit code 1.
- Điều kiện ưu tiên severity nhỏ hơn có thể vẫn đúng; không dùng kết quả này để kết luận contract tổng thể đã đạt.
- Nếu máy không tái hiện, xác nhận .NET SDK 8.x, chưa sửa starter và chạy dotnet run --project starter/BaggageDispatch.csproj -- reproduce từ thư mục unit.
