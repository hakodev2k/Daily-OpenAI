# After — Khi đã sửa

- `verify.ps1` trả exit code 0.
- Cả `acoustic-batch` và `empty-batch` đều có `header=True`, `footer=True`, `roundtrip=True`.
- `reproduce.ps1` không còn phát hiện lỗi và trả exit code khác 0: đây là kết quả đúng sau khi sửa.
- Thử payload lớn hơn và kiểm tra artifact trước khi upload; không khẳng định tính bền vững chỉ dựa vào unit test.
