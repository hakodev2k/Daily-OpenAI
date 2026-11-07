# UNIT-DIAG-001 — Request và background log không nối được cùng operation

## Mục tiêu
Điều tra observability context bị mất khi công việc chuyển từ HTTP request sang background worker.

## Bối cảnh thực tế
API nhận yêu cầu, đưa công việc sang queue nội bộ và trả response. Worker vẫn hoàn thành việc, nhưng log phía worker không thể tra cứu bằng operation ID của request.

## Bạn cần làm gì
Reproduce, thu thập log evidence, ghi hypothesis, sửa boundary truyền metadata tối thiểu, rồi verify.

## Yêu cầu môi trường
.NET 8 SDK.

## Chạy nhanh
`dotnet run --project starter/ReceiptApi`

## Cách reproduce vấn đề
Gửi request có operation ID, sau đó so sánh log request và worker.

## Những gì cần quan sát
Thời điểm request kết thúc, worker bắt đầu, và metadata thực sự được đưa vào work item.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-1.md) · [Hint 2](hints/hint-2.md) · [Hint 3](hints/hint-3.md)

## Reference Solution
⚠️ Spoiler: [solution](solution/README.md)

## Expected Results
Worker log giữ được operation correlation mà không giữ request-owned object.

## Estimated Time
45 phút.
