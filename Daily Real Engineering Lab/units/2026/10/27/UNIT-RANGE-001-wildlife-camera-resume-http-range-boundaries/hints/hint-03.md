# Hint 3

Byte range dùng hai đầu mút inclusive. Đối chiếu cách tính offset cuối với `chunkSize`; sau đó kiểm tra độ dài payload và metadata server trước khi append vào stream.
