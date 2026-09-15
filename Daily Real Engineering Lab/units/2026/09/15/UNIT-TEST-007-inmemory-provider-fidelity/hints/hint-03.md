# Hint 3

Với test cần chứng minh unique index, hãy dùng một relational provider nhẹ chạy local, tạo schema thật từ EF model, rồi assert database từ chối lần ghi duplicate. Đồng thời giữ một happy-path test để tránh biến regression test thành test chỉ biết ném exception.