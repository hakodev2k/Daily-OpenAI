# UNIT-ASP-004 — Webhook Body Disappears After Audit Middleware

## Mục tiêu
Điều tra một lỗi ASP.NET Core request pipeline trong đó webhook hợp lệ bị từ chối sau khi thêm audit middleware.

## Bối cảnh thực tế
Payment provider gửi webhook đã ký. Endpoint hoạt động trước khi team thêm middleware để ghi kích thước payload phục vụ audit. Sau thay đổi, provider retry liên tục dù payload và signature phía provider không đổi.

## Bạn cần làm gì
Reproduce lỗi, thu thập evidence ở middleware và endpoint, viết ít nhất 2 hypotheses, sửa starter và verify rằng audit vẫn hoạt động đồng thời webhook hợp lệ được chấp nhận.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell 7+.

## Chạy nhanh
`./run.ps1`

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trong terminal khác sau khi app đã start.

## Những gì cần quan sát
- HTTP status của webhook.
- Số byte middleware quan sát được.
- `bodyLength` endpoint trả về khi request bị từ chối.
- So sánh cùng payload khi audit middleware có/không tham gia pipeline.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
`hints/hint-01.md`, `hint-02.md`, `hint-03.md`.

## Reference Solution
`solution/README.md` — spoiler, chỉ mở sau khi đã thử fix.

## Expected Results
Before: request hợp lệ bị từ chối và endpoint không quan sát payload như mong đợi. After: audit vẫn đọc được payload, endpoint xác minh cùng payload và trả 200.

## Estimated Time
45 phút.