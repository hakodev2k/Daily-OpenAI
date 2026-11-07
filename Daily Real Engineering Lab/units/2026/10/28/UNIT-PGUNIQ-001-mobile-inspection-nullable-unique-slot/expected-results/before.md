# Before — starter chưa sửa
- reproduce.ps1 kết thúc thành công và in NOTICE REPRODUCED; SELECT hiển thị hai ca lưu động của depot 42 lúc 08:00 UTC.
- Bay 7 lúc 08:00 vẫn không thể tạo lịch trùng.
- verify.ps1 phải thất bại với VERIFY_FAIL: conflicting mobile slot was accepted. Đây là regression test đang đỏ.
- Nếu Docker chưa sẵn sàng, kiểm tra docker compose ps và docker compose logs db; chạy lại script khi container healthy.
