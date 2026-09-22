# UNIT-COSMOS-001 — Partition Query Boundary

## Mục tiêu
Điều tra một read path có kết quả đúng nhưng chi phí và tail latency tăng mạnh khi dữ liệu được phân bố rộng hơn.

## Bối cảnh thực tế
Một Audit API multi-tenant chạy ổn ở môi trường nhỏ. Sau khi số tenant và dữ liệu tăng, endpoint đọc audit của một tenant vẫn trả đúng dữ liệu nhưng diagnostics cho thấy request charge tăng và p95 xấu dần. Team cần tìm nguyên nhân trước khi scale hoặc cache.

## Bạn cần làm gì
1. Chạy starter để thu baseline.
2. Ghi evidence và ít nhất hai hypothesis vào workspace.
3. Chỉnh learner path trong `starter/Program.cs`.
4. Chạy verify để chứng minh correctness vẫn giữ nguyên và query không còn thực hiện công việc dư thừa theo mô hình của lab.
5. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần Azure subscription; lab dùng deterministic local simulator.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script sẽ chạy cùng một tenant-scoped read trên dataset đã chia thành nhiều logical partitions và kiểm tra symptom định trước.

## Những gì cần quan sát
- Số partition được query chạm tới.
- Tổng request-unit mô phỏng.
- Số item được scan so với số item thực sự trả về.
- Correctness của tập audit records.

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
[Spoiler — chỉ mở sau khi đã thử](solution/README.md)

## Expected Results
Trước khi sửa, read path vẫn đúng về dữ liệu nhưng diagnostics cho thấy phạm vi công việc lớn hơn business scope của request. Sau khi sửa, cùng records được trả về với phạm vi query phù hợp hơn và chi phí mô phỏng giảm đáng kể.

## Estimated Time
Khoảng 50 phút.