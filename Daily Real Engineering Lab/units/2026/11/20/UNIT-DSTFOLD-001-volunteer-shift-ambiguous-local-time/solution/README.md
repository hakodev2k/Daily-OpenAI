# Reference Solution — chỉ đọc sau khi tự sửa

## Symptoms
Ca 01:30 ngày 01/11/2026 có hai instant hợp lệ, còn 02:30 ngày 08/03/2026 không tồn tại trong múi giờ mô phỏng.

## Evidence
`IsAmbiguousTime` và `IsInvalidTime` phát hiện các biên; offset -04:00 và -05:00 tương ứng UTC 05:30Z và 06:30Z.

## Root cause
Local `DateTime` thiếu thông tin để định danh instant khi DST fold; việc lấy offset mặc định tự đưa ra quyết định nghiệp vụ. Một offset tùy ý có thể không hợp lệ.

## Why the fix works
Từ chối giờ không tồn tại, yêu cầu offset khi mơ hồ và kiểm tra offset nằm trong tập offset được zone chấp nhận trước khi chuyển UTC. Code mẫu ở `solution/Program.cs`.

## How to verify
Sửa chính `starter/Program.cs` rồi chạy `./verify.ps1`. Sáu assertion phải PASS.

## Alternatives
Yêu cầu ISO 8601 có offset và zone ID, validate ở server; hoặc UI cho chọn lần 01:30 thứ nhất/thứ hai.

## Wrong / misleading fixes
Cộng cố định một giờ chỉ chữa một tình huống; ép `DateTimeKind.Utc` thay đổi nghĩa input; lưu UTC mà không lưu zone làm mất intent cho lịch lặp.

## Production implications
Lưu instant UTC cùng zone ID, local intent và offset đã chọn; version hóa quy tắc xử lý DST cho lịch lặp. Lab dùng custom zone để độc lập OS.

## Trade-offs
Hỏi lại offset tăng thao tác UI nhưng tránh đặt ca sai. Policy earlier/later chỉ hợp lệ nếu được hợp đồng nghiệp vụ chấp thuận.

## Senior engineer should notice
Validation phải chạy trước DB write và notification; regression cần bao phủ fold, gap, normal và offset không hợp lệ.

## References
https://learn.microsoft.com/dotnet/api/system.timezoneinfo.isambiguoustime
https://learn.microsoft.com/dotnet/api/system.timezoneinfo.isinvalidtime
https://learn.microsoft.com/dotnet/api/system.timezoneinfo.getambiguoustimeoffsets
