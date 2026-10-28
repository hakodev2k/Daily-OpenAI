# UNIT-BODY-001 — Webhook intake bị mất payload sau lớp audit

## Mục tiêu
Điều tra luồng đọc HTTP request body trong ASP.NET Core, xác định điều kiện khiến audit và endpoint nhìn thấy nội dung khác nhau, sửa pipeline mà không làm mất tính đúng đắn hay tăng rủi ro vận hành.

## Bối cảnh thực tế
Nền tảng tiếp nhận hồ sơ thú nuôi của nhiều trạm cứu hộ có một webhook POST JSON. Sau khi thêm lớp audit, bản ghi audit vẫn chứa payload đầy đủ nhưng endpoint trả **422** cho một số yêu cầu hợp lệ. Payload trong fixture là dữ liệu synthetic, không chứa PII.

## Bạn cần làm gì
1. Chạy fixture gốc và ghi lại status, độ dài payload tại audit và endpoint.
2. Đề xuất tối thiểu hai giả thuyết, kiểm tra bằng evidence trong terminal.
3. Sửa **starter/RequestAuditMiddleware.cs**; không thay đổi contract endpoint hoặc bỏ audit.
4. Chạy verification trên chính starter đã chỉnh sửa; đánh giá chi phí bộ nhớ, kích thước body và dữ liệu nhạy cảm.

## Yêu cầu môi trường
.NET SDK 8.0 trở lên, có khả năng build ứng dụng target `net8.0`; PowerShell 7+ hoặc Windows PowerShell. Không cần Docker, NuGet bên thứ ba, Azure hay mạng.

## Chạy nhanh
Từ thư mục unit:
```powershell
./run.ps1
./reproduce.ps1
```
Có thể dùng `dotnet run --project starter/RequestBodyReplayLab.csproj -- --run` để xem fixture trực tiếp.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trên starter nguyên bản. Script chỉ thành công khi build thành công **và** fixture phát hiện đúng triệu chứng; lỗi build không được coi là reproduction.

## Những gì cần quan sát
- Status HTTP mô phỏng của hai lần POST hợp lệ.
- Payload được audit ghi nhận so với payload endpoint nhận.
- Khả năng seek của request stream trước và sau middleware.
- Hành vi sau khi sửa đối với hai request độc lập.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ đọc sau khi tự thử. [Giải thích và trade-offs](solution/README.md) · [Mã tham chiếu](solution/RequestAuditMiddleware.cs)

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
65 phút. [Investigation workspace](workspace/my-investigation.md) · [Senior challenge](extensions/senior-challenge.md)
