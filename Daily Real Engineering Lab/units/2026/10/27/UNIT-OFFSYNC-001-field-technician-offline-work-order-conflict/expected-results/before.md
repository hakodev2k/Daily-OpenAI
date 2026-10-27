# Before — starter chưa sửa

`./reproduce.ps1` phải in `REPRODUCED` và exit code 0, đồng thời cho thấy: A-101 Applied, B-201 Ignored, B-202 Applied, B-201 retry Ignored, A-102 Ignored. Final Status=Cancelled; Note=None; Revision=2.

`./verify.ps1` phải trả exit code khác 0. Nếu reproduce không đúng, kiểm tra có đang chạy starter nguyên bản hay không, `dotnet --version`, và lệnh `dotnet run --project ./starter/OfflineSyncLab.csproj -- --reproduce`.
