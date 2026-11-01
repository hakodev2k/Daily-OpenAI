# Trước khi sửa
Chạy `./reproduce.ps1` trên starter ban đầu:
- Lô 12.000 điểm đo có tổng đúng `780000000 Wh`.
- Lô 48.000 điểm đo có tổng kỳ vọng `3120000000 Wh` nhưng tổng thực tế khác kỳ vọng, có thể âm.
- Script in `REPRODUCED` và exit 0 khi triệu chứng tồn tại.
- `./verify.ps1` phải thất bại (exit khác 0) trên starter chưa sửa.

Nếu không reproduce: kiểm tra .NET SDK 8, chạy lại `./setup.ps1`, đảm bảo chưa sửa `starter/EnergySettlement.cs`, và không thay đổi bộ dữ liệu kiểm thử.
