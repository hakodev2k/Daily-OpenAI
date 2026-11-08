# UNIT-DSTWINDOW-001 — Cửa sổ đặt chỗ thư viện lệch một giờ khi chuyển DST

## Mục tiêu
Điều tra và refactor quy tắc mở đặt phòng theo giờ địa phương, đảm bảo kết quả ổn định qua các thay đổi lịch.

## Bối cảnh thực tế
Thư viện tại New York cho phép đặt phòng trước đúng 7 ngày, mở lúc 09:00 theo giờ địa phương. Đội hỗ trợ ghi nhận người dùng thấy nút đặt phòng xuất hiện sớm hơn dự kiến sau một cuối tuần tháng 3. Các lịch mùa đông vẫn đúng.

## Bạn cần làm gì
Chạy hệ thống, lưu evidence, đề xuất ít nhất hai giả thuyết, refactor `starter/BookingWindow.cs`, và kiểm tra các ngày trước/sau chuyển mùa. Không thay đổi hợp đồng public của `BookingWindow.OpenAtUtc`.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+ (hoặc Windows PowerShell).

## Chạy nhanh
Tại thư mục unit: `dotnet run --project starter/BookingLab.csproj -- --inspect`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Lưu các mốc UTC in ra và so sánh với yêu cầu 09:00 New York.

## Những gì cần quan sát
Ghi nhận thời điểm mở theo UTC, thời điểm tương ứng tại địa phương, và chênh lệch theo từng ngày. Phân biệt dữ liệu test với quan sát thực tế.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** Chỉ xem [solution/README.md](solution/README.md) sau khi tự sửa và verify.

## Expected Results
Starter: một ngày kiểm tra lệch 60 phút; sau sửa: các mốc kiểm tra trùng 09:00 theo lịch New York và không thay đổi quy tắc 7 ngày.

## Estimated Time
75 phút.
