# UNIT-WEB-001 — Cache Variation Contract in a Multi-Language Site

## Mục tiêu
Điều tra lỗi nội dung chỉ xuất hiện khi nhiều request khác ngôn ngữ đi qua cùng một shared cache.

## Bối cảnh thực tế
Một product site trả nội dung theo language preference. Backend trả đúng khi gọi trực tiếp, nhưng qua cache đôi lúc người dùng thấy nội dung của request trước đó.

## Bạn cần làm gì
Reproduce, thu evidence về request/response/cache behavior, ghi hypothesis, sửa starter và verify.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell.

## Chạy nhanh
`./reproduce.ps1`

## Cách reproduce vấn đề
Chạy workload có các request cùng URL nhưng khác language preference.

## Những gì cần quan sát
Ghi lại language yêu cầu, nội dung thực nhận, cache hit/miss và thứ tự request. Không suy luận root cause chỉ từ một request đơn lẻ.

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
Xem [before](expected-results/before.md) và [after](expected-results/after.md).

## Estimated Time
50 phút.