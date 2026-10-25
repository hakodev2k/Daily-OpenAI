# Trước sửa

- `./setup.ps1` build starter thành công nếu SDK 8 có sẵn.
- `./reproduce.ps1` kết thúc thành công vì nhận diện đúng triệu chứng có chủ đích.
- Hai `control` khớp `expected`; hai `audit` là `missing`.
- `./verify.ps1` trả lỗi và in `FAIL: audit correlation contract`.

Nếu không reproduce được, kiểm tra SDK bằng `dotnet --info`, xác nhận đang chạy `starter/` và chưa sửa code; không dùng `solution/` cho script reproduce.
