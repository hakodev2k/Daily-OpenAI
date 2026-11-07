# UNIT-CORS-001 — SPA đối tác không gửi được preview bồi thường

## Mục tiêu
Điều tra sự khác biệt giữa HTTP client và browser; thu thập evidence từ request/response và sửa hợp đồng tích hợp mà không mở rộng phạm vi origin hoặc method.

## Bối cảnh thực tế
Cổng đối tác bảo hiểm gửi POST tạo bản preview hồ sơ. API client nhận 200, nhưng SPA tại https://portal.insurer.test báo network error sau khi frontend cập nhật phiên bản. Nhóm vận hành nghi ngờ token, route hoặc cross-origin behavior. Tất cả dữ liệu, origin và token là synthetic.

## Bạn cần làm gì
Reproduce, ghi ít nhất hai hypotheses, so sánh HTTP headers, sửa learner-editable starter và verify cả positive/negative cases.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 5.1+ hoặc 7; không cần Docker, Azure, NuGet ngoài framework, hoặc browser.

## Chạy nhanh
```powershell
.\setup.ps1
.\reproduce.ps1
```
Để chạy API thủ công: `.\run.ps1`.

## Cách reproduce vấn đề
`.\reproduce.ps1` khởi động API loopback và gửi direct POST cùng ba OPTIONS requests mô phỏng browser. Reproduce thành công chỉ chứng minh lỗi starter, không chứng minh đã sửa.

## Những gì cần quan sát
So sánh direct POST, trusted origin preflight, foreign origin và disallowed method. Ghi status cùng các `Access-Control-Request-*` và `Access-Control-Allow-*` headers; status 204 có đủ chứng minh browser chấp nhận không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis tại [workspace](workspace/my-investigation.md).
3. Sửa code trong `starter/`, không sửa probe.
4. Chạy `.\verify.ps1`.
5. Sau đó mới xem reference.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
55 phút, L2 / D4.
