# UNIT-TICKCOALESCE-001 — Bỏ sót cửa sổ quan sát tại ga tàu

## Mục tiêu
Điều tra độ chính xác của lịch lấy mẫu trong một .NET worker; phân biệt số lần poll thành công với số cửa sổ thời gian cần giám sát.

## Bối cảnh thực tế
Hệ thống an toàn sân ga lấy mẫu mật độ người mỗi 100 ms. Khi API camera chậm đột biến, dashboard vẫn báo worker khỏe nhưng báo cáo số lần lấy mẫu thấp hơn số cửa sổ giám sát. Không được suy đoán nguyên nhân chỉ từ một biểu đồ.

## Bạn cần làm gì
Chạy starter, lưu log, lập tối thiểu hai giả thuyết, tìm contract của timer, sửa worker để báo cáo chính xác số slot bị bỏ lỡ và không giả mạo mẫu camera. Giữ cancellation an toàn.

## Yêu cầu môi trường
.NET SDK 8.0.x, PowerShell 7+; không cần cloud, database hay NuGet ngoài framework.

## Chạy nhanh
`dotnet run --project starter/RailPoller.csproj` từ thư mục unit. Dùng `./reproduce.ps1` để khẳng định triệu chứng.

## Cách reproduce vấn đề
`pwsh ./reproduce.ps1`; ghi lại `polls`, `expectedSlots`, và các khoảng cách giữa timestamps. Dữ liệu camera là synthetic.

## Những gì cần quan sát
Có những khoảng thời gian dài hơn chu kỳ danh nghĩa nhưng worker vẫn kết thúc bình thường. So sánh số mẫu thực nhận và số slot thời gian trôi qua; không giả định máy thực thi chính xác đến mili-giây.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trên `starter/Program.cs`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi tự điều tra](solution/README.md)

## Expected Results
Starter: log cho thấy các slot danh nghĩa trôi qua nhưng không tương ứng 1:1 với poll. Sau fix: công khai `missedSlots` và không tạo dữ liệu camera giả; verify kiểm tra bất biến `observedSlots + missedSlots == expectedSlots` trên mô phỏng deterministic.

## Estimated Time
55 phút.
