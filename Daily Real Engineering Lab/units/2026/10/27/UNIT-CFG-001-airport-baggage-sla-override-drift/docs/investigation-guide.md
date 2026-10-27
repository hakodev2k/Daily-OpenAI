# Investigation Guide — không tiết lộ đáp án

## Evidence cần thu thập

- Giá trị appsettings.json của bản build đang chạy.
- Tên và giá trị của hai biến môi trường process-local do reproduce.ps1 cung cấp.
- Output EXPECTED_*, EFFECTIVE_* và exit code.
- Các nguồn cấu hình được đăng ký trong Program.cs; không kết luận chỉ dựa vào việc biến môi trường tồn tại.

## Các giả thuyết cạnh tranh

1. Deployment đặt sai tên biến hoặc sai separator.
2. Application đang chạy với file cấu hình khác file dự kiến.
3. Hệ thống cấu hình chọn một nguồn dữ liệu khác với nguồn vận hành mong đợi.
4. Runtime không đọc lại giá trị sau khi deployment thay đổi.

Hãy đề xuất một phép thử nhỏ để loại trừ từng giả thuyết. So sánh một biến số mỗi lần.

## Phân tích sau sửa

Giải thích tại sao hardcode ngưỡng 5 phút là không đủ. Chứng minh cả default 30 phút và override 5/11 phút; giữ nguyên hành vi queue.
