# Sau khi sửa
- `TECH-17`: Completed=2, Failed=1, Total=3.
- `TECH-42`: Completed=0, Failed=1, Total=1; sau một event thành công tiếp theo là 1/1/2.
- `TECH-99` chưa tồn tại: Total=0.
- Chỉnh sửa một snapshot lấy từ `Read()` không thay đổi state đã lưu.
- Input thiếu TechnicianId bị từ chối.
- `./verify.ps1` trả exit code 0 và in `VERIFIED`.
- `./reproduce.ps1` sau khi sửa không còn reproduce được triệu chứng ban đầu và trả nonzero; đây là kết quả mong đợi.
