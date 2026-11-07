# UNIT-PERF-001 — Large Object Allocation Under Export Load

## Mục tiêu
Điều tra memory pressure của một document export worker khi xử lý batch lớn bằng evidence thay vì tối ưu theo phỏng đoán.

## Bối cảnh thực tế
Worker chạy ổn với batch nhỏ. Khi batch tăng, working set tăng mạnh, pause dài hơn và throughput giảm dù CPU trung bình không cao. Sau khi batch kết thúc, memory giảm chậm.

## Bạn cần làm gì
Chạy starter, reproduce workload, ghi hypotheses từ metrics, sửa learner-editable implementation, rồi chạy verify. Không xem solution trước khi tự thử.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell. Lab dùng deterministic workload local.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
So sánh số document đã xử lý, lượng allocation được báo cáo, peak live bytes và số lần simulated expensive collection giữa batch nhỏ và batch lớn. Xác định evidence nào tăng theo batch size.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
Xem `hints/` theo thứ tự khi cần.

## Reference Solution
Xem `solution/README.md` sau khi đã tự thử.

## Expected Results
Before: peak live data tăng gần tuyến tính theo số document trong batch. After: peak live data được giữ trong một giới hạn nhỏ hơn đáng kể trong khi output count vẫn đúng.

## Estimated Time
60 phút.