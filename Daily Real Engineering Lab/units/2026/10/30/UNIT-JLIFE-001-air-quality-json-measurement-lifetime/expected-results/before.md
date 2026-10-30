# Before — baseline

Chạy `./reproduce.ps1` trên starter chưa sửa.

- Hai case `AQ-HN-017` và `AQ-DN-042` ghi `EXCEPTION=ObjectDisposedException`.
- `PUBLISHED=0/2`, `LAB_RESULT=FAIL`.
- `CASE=INVALID_JSON RESULT=PASS` vì JSON sai cú pháp vẫn bị từ chối đúng.
- `REPRODUCED=YES`, exit code của **script** bằng 0. `dotnet run` bên trong trả 2 để phân biệt lỗi nghiệp vụ với lỗi build.

Nếu không thấy lỗi dự kiến, xác nhận đang dùng starter nguyên bản, đã cài .NET 8 SDK/runtime và kiểm tra build output.
