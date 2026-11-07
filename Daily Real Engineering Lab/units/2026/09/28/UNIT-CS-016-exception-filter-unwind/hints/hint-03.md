# Hint 3

Nếu policy cần state sau cleanup, cân nhắc để `catch` chạy sau unwind rồi mới thực hiện policy, thay vì dùng cleanup-dependent state trong filter.
