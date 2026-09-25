# UNIT-PERF-002 — Large Object Allocation Pressure in Export Pipeline

## Mục tiêu
Điều tra một export pipeline hoạt động đúng về dữ liệu nhưng gây memory spikes và pause tăng khi chạy lặp lại.

## Bối cảnh thực tế
Một internal audit service tạo nhiều export lớn theo batch. Sau khi tăng kích thước dữ liệu, throughput giảm theo thời gian dù CPU trung bình không cao.

## Bạn cần làm gì
Reproduce, ghi hypothesis, quan sát allocation/GC evidence, sửa starter và verify mà không đổi nội dung export.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell.

## Chạy nhanh
`./reproduce.ps1`

## Cách reproduce vấn đề
Chạy script và quan sát summary cho nhiều vòng export.

## Những gì cần quan sát
So sánh lượng allocated bytes, số lần collection và checksum giữa các vòng. Functional output phải giữ nguyên.

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
60 phút.