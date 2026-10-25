# Hint 3 — cơ chế

HMAC hoạt động trên byte sequence. Nếu nhà cung cấp ký raw HTTP body, receiver phải kiểm tra chữ ký trên **chính** byte sequence đó; việc deserialize/serialize có thể thay đổi biểu diễn. Giữ so sánh constant-time và test negative.
