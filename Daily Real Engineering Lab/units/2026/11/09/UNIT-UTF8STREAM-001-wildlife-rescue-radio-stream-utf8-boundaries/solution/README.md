# Reference solution — spoiler

## Root cause
`Encoding.UTF8.GetString(chunk)` là thao tác decode độc lập. Nếu một code point UTF-8 trải qua hai chunk, decoder của lần gọi đầu không giữ trạng thái cho lần gọi tiếp theo; byte bị chuyển thành U+FFFD. TCP không cam kết ranh giới `ReadAsync` trùng ranh giới ký tự hay thông điệp.

## Fix
Giữ một `Decoder` xuyên suốt luồng; decode từng chunk với `flush:false`, rồi `flush:true` đúng một lần ở cuối. Bản mẫu nằm trong [FrameReader.cs](FrameReader.cs). Thay nội dung `starter/FrameReader.cs` bằng bản mẫu sau khi tự thử.

## Verification
`./scripts/verify.ps1` chạy trên **starter learner-editable**, không chạy solution. Test byte-by-byte đặc biệt quan trọng; malformed-final kiểm tra xử lý đoạn UTF-8 dang dở ở EOF.

## Wrong fixes
- `Trim()` / `Replace("�","")`: che mất corruption, không phục hồi byte.
- Đọc buffer lớn hơn: giảm xác suất lỗi nhưng chunk boundary vẫn không được đảm bảo.
- `Encoding.Default`: thay encoding, sai hợp đồng UTF-8.
- Tự nối từng byte thành `char`: sai với Unicode nhiều byte và surrogate pairs.

## Trade-offs
- `Decoder` là stateful và không dùng chung đồng thời giữa các luồng độc lập; tạo một instance trên mỗi message/stream.
- Khi xử lý stream vô hạn, cần giới hạn kích thước message, framing, cancellation và xử lý partial frame; bài này cố tình giới hạn ở một message đã xác định EOF.
- Dữ liệu malformed có thể cần policy strict (`DecoderFallback.ExceptionFallback`) thay vì replacement; phải xác định yêu cầu nghiệp vụ trước khi đổi policy.

