# Trước khi sửa

- `run.ps1` cho thấy ASCII pass nhưng các case Unicode báo lỗi về byte budget, ranh giới hoặc mã hóa.
- `reproduce.ps1` trả exit code 0 khi quan sát ít nhất ba lỗi có chủ đích; không đồng nghĩa starter đã đúng.
- `verify.ps1` trả exit code 1 trước khi sửa.
- Nếu không reproduce: kiểm tra .NET SDK 8, chạy `setup.ps1` rồi xem chi tiết `run.ps1`.
