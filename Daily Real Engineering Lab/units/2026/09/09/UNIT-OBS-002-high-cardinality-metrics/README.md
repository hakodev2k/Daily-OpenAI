# UNIT-OBS-002 — Metrics Cardinality Investigation

## Mục tiêu
Điều tra metrics pipeline .NET có số lượng time series tăng quá nhanh theo traffic, xác định dimension gây cardinality cao và sửa instrumentation mà vẫn giữ khả năng quan sát endpoint-level.

## Bối cảnh thực tế
Một API tra cứu đơn hàng vừa bổ sung custom metrics. Sau load test dài, response vẫn đúng nhưng memory của metrics collector tăng đều và dashboard chậm dần.

## Bạn cần làm gì
1. Chạy `./reproduce.ps1`.
2. Ghi lại request count và metric series count.
3. Xác định dimension làm cardinality tăng.
4. Sửa `starter/`.
5. Chạy `./verify.ps1`.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script mô phỏng 5.000 request qua cùng một logical endpoint với nhiều identifier khác nhau và in total requests, total metric series cùng sample series keys.

## Những gì cần quan sát
- quan hệ giữa request count và series count
- label/tag nào có nhiều giá trị gần như duy nhất
- dimension nào thực sự cần cho dashboard

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
> **Spoiler:** chỉ mở sau khi bạn đã tự reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results
Trước khi sửa, 5.000 request tạo ra rất nhiều metric series. Sau khi sửa, request count vẫn chính xác nhưng series count nhỏ và ổn định theo logical endpoint/status.

## Estimated Time
30–50 phút.
