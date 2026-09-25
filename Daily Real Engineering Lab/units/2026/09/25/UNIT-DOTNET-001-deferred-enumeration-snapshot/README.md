# UNIT-DOTNET-001 — Deferred Enumeration Changes the Batch Snapshot

## Mục tiêu
Điều tra một batch processor mà các phép đo trong cùng operation không mô tả cùng tập dữ liệu.

## Bối cảnh thực tế
Một job chọn invoices đủ điều kiện, ghi audit count, sau đó xử lý batch. Khi nguồn dữ liệu thay đổi giữa các bước, output trở nên khó giải thích.

## Bạn cần làm gì
Reproduce, ghi evidence, đưa ra hypotheses, sửa starter và chạy verify.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell.

## Chạy nhanh
`./reproduce.ps1`

## Cách reproduce vấn đề
Chạy starter và so sánh audit count với danh sách invoice thực sự được xử lý.

## Những gì cần quan sát
Quan sát thời điểm từng giá trị được tạo, dữ liệu nguồn trước/sau bước audit và IDs được xử lý. Không sửa trước khi ghi hypothesis.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler](solution/README.md)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
45 phút.