# Before

- `./reproduce.ps1` exit code 0, in `--reproduce` mode.
- Case 2 nhân viên: `approved=2, remaining=0` — chứng minh vi phạm quy tắc.
- Case 3 nhân viên: `approved=2, remaining=1`.
- Case 1 nhân viên: `approved=0, remaining=1`.
- `./verify.ps1` phải thất bại trên starter gốc; nếu verify thành công ngay từ đầu, kiểm tra lại workspace và các file đã được sửa.
