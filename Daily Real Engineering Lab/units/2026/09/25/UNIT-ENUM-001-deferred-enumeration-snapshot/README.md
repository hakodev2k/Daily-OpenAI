# UNIT-ENUM-001 — Deferred Enumeration Changes the Batch Snapshot

## Mục tiêu
Điều tra batch processor có các phép đo trong cùng operation không mô tả cùng tập dữ liệu.

## Bối cảnh thực tế
Job chọn invoices đủ điều kiện, ghi audit count, rồi xử lý. Nguồn thay đổi giữa các bước và output trở nên khó giải thích.

## Bạn cần làm gì
Reproduce, thu evidence, ghi hypotheses, sửa starter và verify.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell.

## Chạy nhanh
`./reproduce.ps1`

## Cách reproduce vấn đề
Chạy starter và so sánh audit count với IDs thực sự được xử lý.

## Những gì cần quan sát
Theo dõi thời điểm từng giá trị được tạo, nguồn trước/sau bước audit và IDs được xử lý.

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