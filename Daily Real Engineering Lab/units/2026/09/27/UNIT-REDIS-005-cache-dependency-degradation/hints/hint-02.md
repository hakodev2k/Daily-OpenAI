# Hint 2
Quan sát riêng cache read và cache write. Một request có thể đã lấy được dữ liệu hợp lệ từ origin nhưng vẫn fail ở bước sau. Đừng biến mọi exception thành "cache unavailable".
