# UNIT-DOTNET-002 — Worker dần ngừng xử lý sau vài lỗi downstream

## Mục tiêu
Điều tra một lỗi async/concurrency trong background worker khiến throughput giảm dần sau các lỗi downstream, dù process vẫn sống và CPU thấp.

## Bối cảnh thực tế
Một worker đồng bộ giá sản phẩm gọi nhiều downstream API song song. Để bảo vệ dependency, team giới hạn số operation chạy đồng thời. Khi downstream ổn định, worker chạy bình thường. Sau một số request lỗi, các item khỏe mạnh bắt đầu timeout khi chờ quyền xử lý.

## Bạn cần làm gì
1. Chạy starter và reproduce symptom.
2. Ghi ít nhất 2 hypotheses.
3. Theo dõi số operation vào/ra vùng giới hạn concurrency.
4. Xác định invariant nào bị phá vỡ khi một operation thất bại.
5. Sửa code trong `starter/`.
6. Chạy `verify.ps1`.
7. Sau đó mới xem reference solution.

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
- Bao nhiêu operation bắt đầu chạy.
- Bao nhiêu operation hoàn tất hoặc fail.
- Giá trị concurrency permit còn lại sau mỗi batch.
- Healthy operation sau batch lỗi có vào được vùng xử lý hay bị timeout.
- Process có crash hay chỉ mất khả năng tiến triển.

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
**Before:** batch lỗi làm capacity khả dụng giảm; healthy work sau đó không thể chạy đúng hạn.

**After:** lỗi downstream không làm mất capacity vĩnh viễn; healthy work tiếp tục chạy và concurrency limit vẫn được giữ.

## Estimated Time
30–45 phút.
