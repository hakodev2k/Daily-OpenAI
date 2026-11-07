# Before — starter chưa sửa
- `./reproduce.ps1`: ba poll cùng nội dung tạo **3 controller writes**, in `REPRODUCED`, exit code **0**.
- `./verify.ps1`: ít nhất ba contract thất bại: array mới cùng giá trị, caller mutate object được tái sử dụng, và cache bị ảnh hưởng bởi mutation bên ngoài; exit code **1**.
- Một số case thay đổi thật vẫn pass; điều đó không chứng minh thuật toán so sánh đúng.
