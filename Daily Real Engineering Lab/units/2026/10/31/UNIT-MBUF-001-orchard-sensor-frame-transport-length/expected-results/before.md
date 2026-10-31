# Before — Starter

- `reproduce.ps1` trả exit code 0 khi ba bản tin hợp lệ đều bị từ chối.
- Mỗi case in `REJECT`, `reason=transport-length-mismatch`.
- `sentBytes` lớn hơn `declaredBytes` (giá trị cụ thể phụ thuộc packet; ở starter, các capacity là 128, 256, 512).
- `verify.ps1` trả exit code 1 khi starter chưa được sửa.
- Negative case vẫn bị từ chối.

Nếu không reproduce: kiểm tra .NET SDK 8.x, chạy lại `setup.ps1`, xác nhận đang chạy đúng project trong `starter/` và chưa sửa mã.
