# Sau sửa

- Ca sáng: processed=3, worker=3, report=3.
- Ca chiều: processed=1, worker=1, report=1.
- Ca rỗng: 0/0/0.
- verify.ps1 trả exit code 0; reproduce.ps1 không còn thỏa điều kiện lỗi gốc.
- Quy tắc bỏ quét trùng và mã không đọc được giữ nguyên; ca độc lập không rò số liệu.
