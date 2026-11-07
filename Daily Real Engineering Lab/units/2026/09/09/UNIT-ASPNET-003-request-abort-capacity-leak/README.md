# UNIT-ASPNET-003 — Client Abort, Server Work Keeps Occupying Capacity

## Mục tiêu

Điều tra một ASP.NET Core export flow nơi client đã bỏ request nhưng server-side work vẫn tiếp tục giữ concurrency capacity, sau đó sửa boundary cancellation và verify không làm mất operation timeout.

## Bối cảnh thực tế

Một API tạo document export có giới hạn số job chạy đồng thời để bảo vệ downstream report store. Trong production, người dùng thường đóng tab hoặc reverse proxy ngắt request sớm. Monitoring cho thấy CPU thấp nhưng request mới vẫn phải chờ slot trong các burst có nhiều abandoned request.

## Bạn cần làm gì

1. Chạy reproduction harness.
2. Ghi lại `ActiveOperations`, available slots và trạng thái của các request đã bị client hủy.
3. Đưa ra ít nhất 2 hypothesis trước khi đọc hints.
4. Sửa code trong `starter/` để server work kết thúc đúng lifetime nhưng vẫn giữ operation timeout nội bộ.
5. Chạy `verify.ps1`.
6. So sánh với reference solution sau khi tự hoàn thành.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7 hoặc Windows PowerShell
- Không cần database, Docker hoặc Azure subscription

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

Harness khởi chạy hai export request, chờ cả hai chiếm concurrency slots, sau đó mô phỏng client abort. Script kiểm tra trạng thái hệ thống ngay sau cancellation và in evidence ra console.

## Những gì cần quan sát

- request token đã chuyển sang canceled hay chưa
- số operation vẫn active sau khi client abort
- số concurrency slot còn khả dụng
- thời điểm slot được trả lại
- CPU/GC không phải tín hiệu chính của incident này

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

> **Spoiler:** chỉ mở sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Trước khi sửa:

- client cancellation được phát ra
- server work vẫn còn active một khoảng đáng kể
- concurrency slots chưa được trả lại ngay

Sau khi sửa:

- abandoned request giải phóng work nhanh chóng
- operation timeout nội bộ vẫn còn hiệu lực
- concurrency slots được trả lại đúng lifecycle
- verify pass trên learner-editable `starter/`

## Estimated Time

45–70 phút.
