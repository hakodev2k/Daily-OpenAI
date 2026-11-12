# UNIT-HTTPBODY-001 — Webhook hải quan mất payload sau lớp audit

## Mục tiêu
Refactor ASP.NET Core middleware mà vẫn giữ hợp đồng đọc request của downstream endpoint, có bằng chứng HTTP end-to-end.

## Bối cảnh thực tế
Webhook khai báo container đi qua middleware audit rồi đến endpoint xử lý. Sau khi bật audit, một số request JSON hợp lệ bị từ chối; reverse proxy vẫn ghi nhận body đã gửi.

## Bạn cần làm gì
Reproduce, thu thập status/body và giả thuyết, sửa **starter/Program.cs** để audit và endpoint cùng nhận đúng payload. Không tắt audit hay sửa script kiểm tra.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7, cổng loopback trống. Không cần dịch vụ ngoài.

## Chạy nhanh
`pwsh ./run.ps1` để chạy kiểm thử HTTP trên starter.

## Cách reproduce vấn đề
`pwsh ./reproduce.ps1` xác nhận starter không đáp ứng hợp đồng webhook.

## Những gì cần quan sát
HTTP status, phản hồi JSON, chiều dài dữ liệu audit, và vị trí đọc dữ liệu ở từng thành phần. Đề xuất ít nhất hai giả thuyết trước khi sửa.

## Quy tắc làm lab
1. Reproduce trước. 2. Ghi hypothesis. 3. Thử fix. 4. Verify. 5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md)

## Expected Results
Starter bị từ chối; sau sửa, HTTP 200, reference SHIP-42 và auditLength dương. `pwsh ./verify.ps1` phải PASS.

## Estimated Time
45 phút.
