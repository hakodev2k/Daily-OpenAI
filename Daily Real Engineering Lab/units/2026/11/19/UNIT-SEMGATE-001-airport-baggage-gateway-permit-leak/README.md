# UNIT-SEMGATE-001 — Cổng đối soát hành lý dừng nhận yêu cầu sau lỗi đối tác

## Mục tiêu
Điều tra sự suy giảm năng lực xử lý của một gateway .NET, chứng minh bằng số liệu và sửa mà không phá giới hạn concurrency.

## Bối cảnh thực tế
Dịch vụ đối soát hành lý tại sân bay gọi hệ thống đối tác. Một vài request nhận lỗi 503; sau đó request hợp lệ cũng bắt đầu timeout dù CPU thấp và đối tác đã phục hồi.

## Bạn cần làm gì
Chạy mô phỏng, ghi lại kết quả từng request, lập ít nhất hai giả thuyết, sửa code trong `starter/Program.cs`, rồi kiểm chứng bằng `verify.ps1`.

## Yêu cầu môi trường
.NET SDK 8.0.4xx, PowerShell 7 hoặc Windows PowerShell; không cần Docker, cloud hay NuGet ngoài SDK.

## Chạy nhanh
Từ thư mục lab: `pwsh -File ./reproduce.ps1` (hoặc `./reproduce.ps1` trong PowerShell).

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` với starter chưa sửa; ghi lại `results` và `availablePermits`. Chạy lại để kiểm tra tính ổn định.

## Những gì cần quan sát
Phân biệt lỗi từ đối tác với timeout nội bộ. So sánh số request hoàn tất, giới hạn xử lý song song và tài nguyên còn khả dụng sau mỗi chuỗi. Không kết luận chỉ từ CPU.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng script riêng.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** [Xem sau khi tự thử](solution/README.md).

## Expected Results
Starter: một số request thành công, lỗi đối tác xuất hiện, và request hợp lệ về sau có thể timeout. Sau fix: các lỗi đối tác vẫn được báo chính xác, request hợp lệ tiếp tục chạy, tài nguyên trở về mức ban đầu; `verify.ps1` trả mã 0.

## Estimated Time
55 phút.
