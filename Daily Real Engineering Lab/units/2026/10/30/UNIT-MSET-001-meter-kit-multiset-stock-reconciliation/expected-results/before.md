# Before
Trên starter chưa sửa, `./reproduce.ps1` in `REPRODUCED` và exit 0: manifest `[KIT-A,KIT-A,KIT-B]`, scan `[KIT-A,KIT-B]`, nhưng báo cáo `Missing` rỗng. `./verify.ps1` exit 1. Nếu không tái hiện, kiểm tra `dotnet --version` và chạy từ thư mục unit.
