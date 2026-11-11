# Hint 3
Trong HTTP, strong ETag + If-Match giúp kiểm tra phiên bản tại server. Khi không khớp, trả 412 Precondition Failed. Trong mô phỏng, so sánh Revision trong cùng critical section với thao tác ghi.
