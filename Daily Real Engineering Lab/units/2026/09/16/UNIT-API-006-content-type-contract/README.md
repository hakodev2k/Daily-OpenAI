# UNIT-API-006 — Successful response, unusable representation

## Mục tiêu
Điều tra một API integration trả HTTP 200 nhưng consumer không xử lý được representation theo contract.

## Bối cảnh thực tế
Một service tích hợp với downstream profile API. Sau một deployment, status code vẫn 200 và payload nhìn giống JSON khi đọc log, nhưng typed client bắt đầu đi vào failure path.

## Bạn cần làm gì
Chạy starter, quan sát response metadata và consumer behavior, ghi hypothesis, sửa producer/consumer boundary phù hợp rồi verify mà không làm lỏng contract một cách tùy tiện.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+.

## Chạy nhanh
`./reproduce.ps1`

## Cách reproduce vấn đề
Starter tự host một downstream local và một typed consumer, sau đó thực hiện request qua contract hiện tại.

## Những gì cần quan sát
So sánh HTTP status, response headers, payload bytes và điểm consumer từ chối response.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi evidence và hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó xem solution.

## Hints
`hints.md`

## Reference Solution
`solution/README.md` — spoiler.

## Expected Results
Trước fix, transport thành công nhưng representation contract thất bại. Sau fix, producer và consumer thống nhất contract và regression check vẫn giữ strictness cần thiết.

## Estimated Time
30–45 phút.