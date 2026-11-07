# Before — baseline / pilot

- K1 được xác thực trước 09:30; K2 được chấp nhận từ 09:00.
- E103 và E105 chưa có business transition do lần đầu 504/503; các retry K1 sau cutoff bị 401.
- E106 retry 200 nhưng không tạo transition thứ hai.
- E107 sai signature và E108 timestamp vượt future skew phải bị từ chối.
- HTTP 2xx cao không chứng minh đủ tất cả event hợp lệ đã được xử lý.
- Nếu chỉ giữ K1 lâu hơn để cứu E103/E105, hard cutoff của Security không còn đạt.
