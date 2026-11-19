# UNIT-LOCKORDER-001 — Quầy vé bảo tàng ngừng quyết toán khi chạy song song

## Mục tiêu
Tái hiện sự cố treo tiến trình, thu thập bằng chứng về tiến độ và đề xuất thay đổi bảo đảm tính đúng đắn khi có nhiều thao tác đồng thời.

## Bối cảnh thực tế
Hệ thống bán vé bảo tàng xử lý đồng thời nghiệp vụ chuyển vé giữa hai quầy. Khi chỉ một giao dịch chạy, quyết toán hoàn tất. Khi hai chiều chuyển được xử lý cùng lúc, hệ thống có thể ngừng phản hồi, không có exception.

## Bạn cần làm gì
Chạy tình huống gốc, ghi ít nhất hai hypothesis, xác định tài nguyên đang chờ, sửa `starter/Program.cs` và chứng minh giao dịch vẫn bảo toàn tổng số vé.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell; không cần Docker, database hay Azure.

## Chạy nhanh
Tại thư mục unit: `pwsh ./reproduce.ps1` (hoặc `powershell -File ./reproduce.ps1`). Sau khi sửa: `pwsh ./verify.ps1`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script chạy chương trình gốc với giới hạn 5 giây, ghi nhận tiến trình không kết thúc. Dùng `dotnet run --project ./starter/TicketSettlement.csproj --configuration Release` để tự quan sát (Ctrl+C để dừng).

## Những gì cần quan sát
So sánh hai dòng bắt đầu chuyển vé với sự vắng mặt của dòng `SETTLED`. Ghi lại trạng thái tiến trình và tài nguyên được truy cập. Timeout ở đây là giới hạn của harness, không phải bằng chứng CPU quá tải.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải tham khảo](solution/README.md).

## Expected Results
Trước: hai giao dịch không kết thúc trong cửa sổ 5 giây. Sau: cả hai hoàn tất, không âm số vé, tổng vé trước/sau bằng nhau; kết quả có `SETTLED total=200`.

## Estimated Time
50–65 phút.

