# Senior challenge
Thiết kế receiver cho TCP stream chứa nhiều message có prefix 4-byte length (network byte order), giới hạn 64 KiB/message. Định nghĩa policy cho truncated frame, invalid UTF-8, cancellation, partial read, backpressure và log không chứa PII. Viết bảng failure-injection test và chứng minh framing độc lập với các lần `ReadAsync`.
