# After — learner fix
- Cả sáu case `PASS` với cùng một service instance xuyên suốt các lần reload.
- Quota giảm xuống 2 phải từ chối request 3; quota tăng lên 5 phải chấp nhận request 5.
- `dotnet run` exit 0, marker `VERIFIED: all six live-policy contract checks passed`.
- `verify.ps1` exit 0 và xác nhận đang kiểm tra `starter/`, không phải `solution/`.
- Giải thích được consistency boundary nếu policy đổi giữa batch đang chạy.
Đây là contract dự kiến; chạy tại máy để lấy bằng chứng thực tế.
