# UNIT-PERF-001 — Allocation Pressure in a Hot Request Path

## Mục tiêu
Điều tra một API có throughput giảm và tail latency tăng khi chạy tải ổn định, dựa trên evidence thay vì tối ưu theo cảm tính.

## Bối cảnh thực tế
Pricing API hoạt động đúng chức năng. Khi traffic tăng, p95/p99 xấu dần dù CPU chưa bão hòa và downstream dependency ổn định. Metrics cho thấy runtime activity thay đổi đáng kể theo request rate.

## Bạn cần làm gì
1. Chạy starter và reproduce symptom.
2. Thu thập output về requests, allocated bytes và collection activity.
3. Ghi ít nhất ba hypothesis trước khi sửa.
4. Xác định phần request path tạo pressure không cần thiết.
5. Sửa code trong `starter/` mà không thay đổi functional result.
6. Chạy `verify.ps1`, rồi mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Tổng allocated bytes qua workload cố định.
- Số Gen0 collections trước/sau workload.
- Functional checksum của output.
- Quan hệ giữa request volume và memory pressure.

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
[Reference Solution — spoiler](solution/README.md)

## Expected Results
Before: workload tạo lượng allocation cao so với dữ liệu đầu vào và có runtime collection activity rõ rệt.

After: cùng functional checksum nhưng allocation giảm đáng kể; verification dùng tỷ lệ thay vì timing tuyệt đối.

## Estimated Time
60 phút.