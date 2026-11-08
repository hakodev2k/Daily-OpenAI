# Reference Solution — SPOILER
Chỉ đọc sau khi reproduce và tự thử fix.

## Root cause
Code giả định New York luôn UTC-05:00. Sau DST mùa xuân, 09:00 America/New_York tương ứng 13:00 UTC, không phải 14:00 UTC. Sau DST mùa thu, offset quay lại UTC-05:00.

## Cách sửa
Thay nội dung `starter/BookingWindow.cs` bằng mã trong [BookingWindow.cs](BookingWindow.cs), rồi chạy `./verify.ps1`. Reference không phải đối tượng mà verify script mặc định kiểm tra.

## Evidence / kết quả
Bốn ngày test bao phủ trước/sau DST mùa xuân và mùa thu; mọi dòng phải PASS. `DateOnly.AddDays(-7)` biểu diễn lịch ngày địa phương, sau đó 09:00 được ánh xạ qua zone rule của đúng ngày mở.

## Wrong fixes
- Thay hằng số -5 thành -4: sửa mùa hè nhưng làm sai mùa đông.
- Dùng offset của `DateTimeOffset.Now`: kết quả phụ thuộc thời điểm chạy, không phụ thuộc lịch mở.
- Cộng/trừ 168 giờ từ UTC của ngày sử dụng phòng: 7 ngày lịch địa phương không đồng nghĩa 168 giờ qua DST.
- Dùng `DateTime.ToUniversalTime()` trên local time không gắn zone: phụ thuộc múi giờ máy chạy.

## Trade-offs
09:00 không nằm trong khoảng DST invalid/ambiguous tại New York hiện tại, nhưng API tổng quát cho các giờ khác phải định nghĩa policy rõ ràng. Reference chọn fail-fast nếu giờ địa phương invalid hoặc ambiguous thay vì tự suy đoán. Dùng tzdb/Windows time zone database của hệ điều hành; dữ liệu quy tắc có thể cập nhật theo pháp luật.

## Thử thách
Bổ sung test ngày leap year, đổi thành 01:30 tại ngày chuyển DST mùa thu, và thiết kế policy khi ambiguous.
