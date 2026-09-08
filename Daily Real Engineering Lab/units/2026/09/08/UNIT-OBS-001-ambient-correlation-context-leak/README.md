# UNIT-OBS-001 — Correlation ID bị rò sang operation kế tiếp

## Mục tiêu
Điều tra một lỗi observability trong code async: log của operation mới đôi khi mang `CorrelationId` của operation trước dù business result vẫn đúng.

## Bối cảnh thực tế
Một background processor dùng ambient context để enrich log. Sau khi thêm helper nhằm tránh phải truyền `CorrelationId` qua nhiều method, dashboard bắt đầu ghép log của các job khác nhau vào cùng một trace.

## Bạn cần làm gì
1. Chạy starter.
2. Reproduce symptom.
3. Ghi ít nhất 2 hypotheses.
4. Xác định lifetime thực tế của correlation context.
5. Sửa `starter/` để mỗi logical operation chỉ nhìn thấy context của chính nó.
6. Đảm bảo cleanup đúng cả success và exception path.
7. Chạy `verify.ps1`.
8. Sau đó mới xem solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 5.1+ hoặc PowerShell 7+

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Operation A đặt correlation `corr-A`.
- Operation B không đặt correlation mới.
- Log của B có nhìn thấy `corr-A` hay không.
- State ambient được clear/restore ở thời điểm nào.
- Business result có thể đúng trong khi telemetry sai.

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
> Spoiler: chỉ xem sau khi đã tự thử.
- [Reference Solution](solution/README.md)

## Expected Results
**Before:** operation B nhìn thấy correlation của operation A.

**After:** mỗi operation chỉ thấy correlation thuộc scope của chính nó; state cũ không rò sang operation tiếp theo.

## Estimated Time
30–45 phút.
