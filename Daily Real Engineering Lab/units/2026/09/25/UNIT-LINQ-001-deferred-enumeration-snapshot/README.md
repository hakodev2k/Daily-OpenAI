# UNIT-LINQ-001 — Deferred Enumeration Breaks Batch Snapshot

## Mục tiêu
Điều tra batch processor có preview và dữ liệu xử lý không nhất quán.

## Bối cảnh thực tế
Job chọn invoice đủ điều kiện, ghi preview, rồi source thay đổi trước khi batch được xử lý.

## Bạn cần làm gì
Reproduce, thu evidence, ghi hypothesis, sửa starter và verify.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell.

## Chạy nhanh
`./reproduce.ps1`

## Cách reproduce vấn đề
Chạy starter và so sánh `PreviewIds` với `ProcessedIds`.

## Những gì cần quan sát
Theo dõi thời điểm selection được đọc, source state và số lần pipeline được enumerate.

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
Xem [before](expected-results/before.md) và [after](expected-results/after.md).

## Estimated Time
45 phút.