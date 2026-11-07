# Concepts after lab
- HTTP body là luồng byte có trạng thái, không phải chuỗi có thể đọc lại tùy ý.
- `StreamReader(leaveOpen:true)` bảo vệ quyền sở hữu stream nhưng không điều chỉnh cursor.
- `EnableBuffering` có thể dùng `FileBufferingReadStream`; buffering có giới hạn và có chi phí memory/disk.
- Rewind phải xảy ra trước `next(context)` khi downstream cần đọc từ đầu.
- Verification cần chứng minh cả audit lẫn endpoint giữ đúng dữ liệu, không chỉ HTTP 200.
- Production cần data minimization, redaction, size caps, exception policy và telemetry không lộ PII.
