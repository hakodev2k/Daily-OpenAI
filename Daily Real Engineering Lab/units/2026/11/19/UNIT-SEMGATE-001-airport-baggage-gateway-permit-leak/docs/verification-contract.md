# Verification contract

- `reproduce.ps1` chạy starter nguyên trạng và ghi triệu chứng, không yêu cầu pass.
- `verify.ps1` chạy chính `starter/Program.cs` sau khi học viên sửa; không chạy reference solution.
- `--verify` yêu cầu kết quả đúng cho bốn request và số permit phục hồi bằng 2.
- Mô phỏng sử dụng lỗi cố định và delay 10 ms; timeout 250 ms nhằm phân biệt request bị chờ vô hạn với request bình thường.
- Không kiểm chứng race dưới tải thực tế; trong production cần stress test, cancellation test và metrics.
