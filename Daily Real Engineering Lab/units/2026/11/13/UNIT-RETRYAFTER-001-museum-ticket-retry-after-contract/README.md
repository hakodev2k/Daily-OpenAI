# UNIT-RETRYAFTER-001 — Cổng bán vé bảo tàng phản hồi 429 bất thường

## Mục tiêu
Điều tra một hợp đồng HTTP bị lệch khi gateway chuyển phiên bản, phân biệt tín hiệu quá tải với chính sách retry, và chứng minh hành vi bằng test có đồng hồ cố định.

## Bối cảnh thực tế
Dịch vụ bán vé trực tuyến nhận HTTP 429/503 từ đối tác thanh toán. Một số phiên bị retry quá sớm; dashboard cho thấy spike 429 dù số request ban đầu không tăng. Sau nâng cấp gateway, log ghi `Retry-After` với nhiều biểu diễn khác nhau.

## Bạn cần làm gì
Reproduce trước; lưu kết quả từng case; viết ít nhất hai giả thuyết; kiểm tra hợp đồng phản hồi và đề xuất thay đổi nhỏ nhất không gây busy-loop. Giữ nguyên public signature `RetryPolicy.Delay`.

## Yêu cầu môi trường
.NET SDK 8.x; không cần mạng, tài khoản hay dịch vụ cloud.

## Chạy nhanh
```powershell
pwsh ./scripts/reproduce.ps1
```
Hoặc `dotnet run --project ./starter/TicketGateway/TicketGateway.csproj`.

## Cách reproduce vấn đề
Chạy script, ghi lại các case PASS/FAIL và giá trị delay thực tế. Sau đó kiểm tra phản hồi giả lập trong `Program.cs` và đối chiếu tiêu chuẩn HTTP. Không thay test case để làm xanh kết quả.

## Những gì cần quan sát
Các request bị từ chối có cùng status nhưng lịch retry khác nhau. Kiểm tra delay âm, delay quá lớn, thời gian hiện tại, và các status không được retry.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/investigation.md`.
3. Thử fix ở starter.
4. Verify bằng `scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Xem gợi ý theo cấp độ](hints.md).

## Reference Solution
[Cảnh báo spoiler: lời giải và phân tích](solution/reference.md).

## Expected Results
7/7 case PASS, không sleep thật, không thay đổi test để che lỗi. Mô tả ranh giới trách nhiệm giữa parsing, retry budget và rate limiting.

## Estimated Time
40–50 phút.
