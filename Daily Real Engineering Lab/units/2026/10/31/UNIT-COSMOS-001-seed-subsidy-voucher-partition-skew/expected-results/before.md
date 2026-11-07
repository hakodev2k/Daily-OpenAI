# Before — Starter

Chạy `pwsh -NoProfile -File ./run.ps1`: với policy mặc định, bucket lớn nhất nhận 900/1.000 voucher (90%); replay vẫn định tuyến ổn định nhưng API chỉ biết voucher ID không suy ra được cooperative partition. `reproduce.ps1` phải in `REPRODUCED` và trả exit code 0 khi thấy lỗi kỳ vọng. Nếu lệnh không chạy được, kiểm tra `pwsh --version`, vị trí unit và JSON policy.
