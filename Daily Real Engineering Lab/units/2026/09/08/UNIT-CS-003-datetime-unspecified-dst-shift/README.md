# UNIT-CS-003 — Lịch gửi notification lệch một giờ sau DST

## Mục tiêu
Điều tra một lỗi temporal correctness trong .NET: cùng một giờ nghiệp vụ được lưu bằng `DateTime` nhưng bị diễn giải sai múi giờ, khiến thời điểm thực thi lệch một giờ quanh thời điểm Daylight Saving Time thay đổi.

## Bối cảnh thực tế
Một hệ thống notification lưu lịch theo giờ địa phương của khách hàng. Scheduler chuyển lịch đó sang UTC trước khi enqueue. Với khách hàng ở `America/New_York`, job chạy đúng hầu hết thời gian nhưng bắt đầu lệch sau khi DST đổi.

## Bạn cần làm gì
1. Chạy starter và reproduce symptom.
2. Ghi ít nhất 2 hypotheses trước khi sửa.
3. Xác định `DateTime.Kind`, timezone business và instant thực tế đang bị trộn ở đâu.
4. Sửa `starter/` để biểu diễn thời điểm không còn phụ thuộc vào timezone ngầm của process.
5. Chạy `verify.ps1`.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 5.1+ hoặc PowerShell 7+

## Chạy nhanh
```powershell
./run.ps1
```

## Reproduce
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Business requirement: chạy lúc `09:00` giờ New York.
- Ngày trước và sau DST có UTC offset khác nhau.
- `DateTime` không tự mang theo timezone business.
- Một conversion dùng timezone ngầm có thể tạo UTC instant sai dù clock time nhìn vẫn đúng.

## Expected behavior
Scheduler phải tạo đúng UTC instant tương ứng với `09:00 America/New_York` cho từng ngày.

## Actual behavior
Starter coi wall-clock time là local time của process rồi convert sang UTC, làm kết quả sai khi timezone business khác timezone máy chạy.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
> Spoiler: chỉ xem sau khi đã tự thử.
- [Reference Solution](solution/README.md)

## Estimated Time
30–45 phút.
