# UNIT-EFCONFLICT-001 — Hai yêu cầu giữ hàng cùng ghi đè phiên bản tồn kho

## Mục tiêu
Điều tra tính đúng đắn khi hai yêu cầu đặt chỗ cùng đọc một bản ghi, xây dựng bằng chứng và kiểm tra bất biến sức chứa.

## Bối cảnh thực tế
Một rạp có đúng một ghế trống. Hai khách đặt gần như đồng thời. Hệ thống trả lời thành công cho cả hai, nhưng báo cáo cuối ngày vẫn ghi còn 0 ghế. Đội vận hành nghi ngờ lỗi từ cache, hàng đợi hoặc giao dịch.

## Bạn cần làm gì
Chạy replay, ghi lại số đặt chỗ thành công, giải thích bất biến nghiệp vụ, đưa giả thuyết và sửa code trong `starter/Program.cs` để không chấp nhận quá sức chứa. Phân tích cách chuyển thiết kế sang EF Core với nhiều process.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell; không cần database hay NuGet ngoài SDK.

## Chạy nhanh
`./run.ps1`

## Cách reproduce vấn đề
`./reproduce.ps1` chạy hai yêu cầu đọc trước khi ghi, mô phỏng interleaving cố định.

## Những gì cần quan sát
Ghi lại số yêu cầu được chấp nhận, số ghế còn lại và số lần thay đổi phiên bản. Phân biệt dữ liệu cuối cùng với tổng số giao dịch đã xác nhận.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md) — chỉ xem sau khi thử sửa.

## Expected Results
Starter: hai xác nhận có thể vượt quá sức chứa dù remaining không âm. Fixed: chỉ một xác nhận; remaining = 0. Không dựa vào timing của máy.

## Estimated Time
50 phút.
