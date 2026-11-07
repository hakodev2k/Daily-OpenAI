# Hint 3

`ReadAllAsync()` kết thúc khi channel vừa hết dữ liệu vừa được đánh dấu hoàn tất. Hãy đặt completion tại boundary chỉ chạy sau khi toàn bộ producer hợp lệ đã kết thúc.