# Sau khi sửa
- `scripts/verify.ps1` trả exit code 0, in `VERIFIED all contracts` và tất cả contract PASS.
- W-17 vẫn CLOSED version 3 sau khi nhận lại version 2.
- Duplicate event không làm tăng số lần áp dụng; conflict không thay thế trạng thái hiện hành.
- W-18 độc lập và nhận version 1.
- `scripts/reproduce.ps1` không còn trả exit code 0 sau khi đã sửa; đó là điều đúng vì triệu chứng gốc không còn tồn tại.
- Chứng minh bằng ADR rằng bản in-memory chỉ mô phỏng semantics, chưa cung cấp atomicity/durability khi chạy đa instance.
