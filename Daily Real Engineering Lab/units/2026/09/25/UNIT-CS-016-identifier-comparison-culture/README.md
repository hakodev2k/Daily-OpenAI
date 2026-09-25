# UNIT-CS-016 — Identifier Lookup Changes Across Server Culture

## Mục tiêu
Điều tra một lookup hoạt động trên máy phát triển nhưng trả kết quả khác khi process chạy với culture khác.

## Bối cảnh thực tế
Dịch vụ invoice routing chọn template bằng identifier do hệ thống khác gửi sang. Sau một thay đổi môi trường, một số identifier hợp lệ không còn match dù dữ liệu cấu hình không đổi.

## Bạn cần làm gì
Reproduce hiện tượng, ghi hypothesis, xác định contract phù hợp cho identifier, sửa starter và chạy verification.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell.

## Chạy nhanh
`./reproduce.ps1`

## Cách reproduce vấn đề
Chạy script reproduce và ghi lại culture, identifier đầu vào và kết quả lookup.

## Những gì cần quan sát
So sánh cùng một logical identifier khi process culture thay đổi. Tập trung vào behavior có thể đo được, chưa đọc solution trước khi hình thành hypothesis.

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
[Spoiler — chỉ xem sau khi tự làm](solution/README.md)

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
45 phút.
