# Before — starter

- `./reproduce.ps1` chạy immutable baseline, in `status=SUCCESS` và `planned=3/6`, kết thúc với `REPRODUCED` và exit code 0.
- `./run.ps1` trên starter chưa sửa hiển thị triệu chứng tương tự.
- `./verify.ps1` trên starter chưa sửa thất bại (ít nhất cycle và unknown prerequisite; duplicate ID gây exception được harness ghi nhận).
- Không yêu cầu timing chính xác; đây là lỗi logic xác định được từ fixture.
- Nếu không reproduce: xác nhận .NET SDK, chạy `dotnet --info`, kiểm tra đường dẫn unit và không thay đổi `baseline/`.
