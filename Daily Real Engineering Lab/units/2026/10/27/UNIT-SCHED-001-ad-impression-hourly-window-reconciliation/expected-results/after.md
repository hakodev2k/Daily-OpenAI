# After — Bản sửa

- `./verify.ps1` trả mã 0 với thông báo PASS.
- Một wake-up xử lý đúng 5 cửa sổ theo thứ tự 01:00 → 05:00.
- Wake-up lặp lại tại cùng `now` không tạo bản ghi trùng.
- Lỗi giả lập ở 03:00 không làm checkpoint vượt quá 02:00.
- Retry tiếp tục từ 03:00 và hoàn tất 03:00–05:00.

**Giới hạn:** lab chỉ kiểm chứng checkpoint trong bộ nhớ. Production cần lưu checkpoint bền vững và idempotency ở downstream; không suy diễn exactly-once từ test này.