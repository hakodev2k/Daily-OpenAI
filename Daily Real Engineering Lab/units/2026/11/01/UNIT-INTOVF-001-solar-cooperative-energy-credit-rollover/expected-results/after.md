# Sau khi sửa
Chạy `./verify.ps1` trên **starter đã sửa**:
- 0 điểm đo → 0 Wh.
- 12.000 điểm đo → 780.000.000 Wh.
- 33.038 điểm đo → 2.147.470.000 Wh.
- 33.039 điểm đo → 2.147.535.000 Wh.
- 48.000 điểm đo → 3.120.000.000 Wh.
- Lặp lại cùng dữ liệu cho cùng kết quả, không làm thay đổi dữ liệu.
- Số đo âm vẫn bị từ chối bằng `ArgumentOutOfRangeException`.
- In `PASS` và exit 0; `reproduce.ps1` không còn báo `REPRODUCED`.

Các số là giá trị toán học cố định, không phụ thuộc tốc độ máy.
