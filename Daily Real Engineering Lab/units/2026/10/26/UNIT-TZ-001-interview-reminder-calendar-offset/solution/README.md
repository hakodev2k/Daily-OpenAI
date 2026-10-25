# Reference Solution — chỉ đọc sau khi tự điều tra

## 1. Symptoms
Đợt phỏng vấn 09:00 tại New York: ngày đầu mỗi đợt đúng, một số ngày sau được lưu thành UTC lệch một giờ. Đây là sai lệch lịch nghiệp vụ, không phải chậm xử lý email.

## 2. Evidence
Mốc 2026-03-07 09:00 là 14:00Z, còn 2026-03-08 và 03-09 09:00 là 13:00Z. Ngày 2026-10-31 09:00 là 13:00Z, nhưng 11-01 và 11-02 09:00 là 14:00Z. So sánh sáu case trong `Program.cs` và `evidence/observations.md`.

## 3. Root cause
`ScheduleBuilder.Build` lấy UTC offset từ **ngày đầu batch** rồi áp dụng cho toàn bộ ngày sau. Offset thay đổi theo timezone rules khi qua ranh giới daylight saving time; một offset cố định không phải timezone.

## 4. Why the fix works
Tạo `DateTime` với `DateTimeKind.Unspecified` cho từng ngày 09:00, rồi chuyển bằng `TimeZoneInfo.ConvertTimeToUtc(local, zone)` trên **mỗi occurrence**. Reference code còn từ chối local time ambiguous/invalid để tránh silently chọn sai policy nếu giờ phỏng vấn thay đổi.

## 5. How to verify
Từ thư mục unit, sao lưu file đã sửa của bạn trước. Sau đó (chỉ để so sánh) thay `starter/ScheduleBuilder.cs` bằng nội dung `solution/ScheduleBuilder.cs`; chạy `./scripts/verify.ps1`. Verification luôn thực thi **starter**, không tự động chuyển sang solution. `verify` phải exit 0 với đủ sáu case. `reproduce` ban đầu exit 0 và sau khi fix không còn reproduce.

## 6. Alternative fixes
- Tính offset bằng `zone.GetUtcOffset(local)` riêng cho mỗi occurrence rồi dựng `DateTimeOffset`: hợp lệ cho giờ 09:00 của case này, nhưng cần policy rõ ràng với ambiguous/invalid time.
- Lưu UTC instant tại thời điểm tạo từng lịch: tốt cho một cuộc hẹn đã chốt; với lịch lặp và thay đổi timezone rules, phải xác định chính sách tái tính toán.
- Sử dụng thư viện như Noda Time khi nghiệp vụ thời gian phức tạp: biểu diễn local time / zone / instant rõ hơn nhưng tăng dependency và learning curve.

## 7. Wrong or tempting fixes
- Cộng/trừ 60 phút cho ngày tháng 3: không tổng quát, sẽ sai khi chuyển mùa theo chiều ngược lại và khi quy tắc vùng thay đổi.
- Dùng timezone của máy chạy worker: môi trường CI/production có thể chạy UTC hoặc vùng khác.
- Đổi mọi giờ local sang `DateTimeKind.Utc`: gắn nhãn không tương đương chuyển đổi.
- Dùng một `DateTimeOffset` cố định cho lịch lặp: offset không mã hóa quy tắc vùng.

## 8. Production implications
Lưu timezone ID (ưu tiên IANA), local appointment contract, UTC instant đã resolve, và audit decision nếu cần. Giám sát mismatch giữa intended local wall-clock và scheduled UTC; thêm tests quanh DST của những vùng phục vụ. Đảm bảo database/timezone-data cập nhật nhất quán giữa các worker.

## 9. Trade-offs
Resolve-on-write làm UTC execution rõ ràng nhưng cần xử lý khi quy tắc timezone đổi sau khi lịch được tạo. Resolve-near-execution phản ánh timezone database mới nhưng có thể thay đổi một lịch đã xác nhận. Product phải quyết định tính bất biến của appointment.

## 10. What a Senior engineer should notice
Business contract là **09:00 local tại named timezone**, không phải một UTC offset. Cần phân biệt local calendar scheduling với elapsed-time scheduling; xử lý ambiguous/invalid times bằng policy, test độc lập ở các ranh giới và audit các thay đổi quy tắc thời gian.
