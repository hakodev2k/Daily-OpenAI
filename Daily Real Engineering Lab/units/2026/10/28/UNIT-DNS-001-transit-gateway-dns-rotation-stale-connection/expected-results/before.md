# Trước khi sửa
- First: EAST; after cutover: EAST; next: EAST.
- Route snapshot đã chuyển WEST nhưng connection attempts thường chỉ là 1.
- `./reproduce.ps1` exit code 0 (đã xác nhận symptom); `./verify.ps1` exit code 1 (chưa đạt hợp đồng mới).
- Nếu số connection attempts khác 1 do môi trường: điều tra reuse và socket lifecycle; không suy diễn từ timing tuyệt đối.
