# Before — baseline bất biến

Chạy `./scripts/reproduce.ps1`:
- Thấy `REPRODUCED` và exit code 0 khi ít nhất bốn case lỗi trọng yếu xuất hiện.
- Các request đơn giản vẫn thành công, nên không thể kết luận toàn bộ API hỏng.
- `./scripts/verify.ps1` với starter chưa sửa phải trả exit code 1, vì nhiều behavior contract không đạt.
- Không phụ thuộc tốc độ máy hoặc network.
