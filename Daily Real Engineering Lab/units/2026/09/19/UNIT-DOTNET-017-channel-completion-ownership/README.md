# UNIT-DOTNET-017 — Telemetry Worker Never Finishes Shutdown

## Mục tiêu
Điều tra một lỗi lifecycle trong pipeline async dùng `System.Threading.Channels`, dựa trên trạng thái task và timeline thay vì đoán từ triệu chứng timeout.

## Bối cảnh thực tế
Một service gom telemetry từ hai producer vào một consumer để flush theo batch. Xử lý bình thường và mọi event đều được ghi, nhưng khi host shutdown sau khi producer kết thúc, consumer không hoàn tất và deployment phải chờ timeout.

## Bạn cần làm gì
Reproduce shutdown stall, ghi hypothesis, xác định contract lifecycle còn thiếu, sửa `starter/` và chứng minh pipeline kết thúc tự nhiên mà không làm mất event.

## Yêu cầu môi trường
- .NET SDK 8.0.x
- PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script chạy starter với timeout ngắn và xác nhận tất cả event đã được xử lý nhưng pipeline vẫn chưa hoàn tất.

## Những gì cần quan sát
- Số event producer đã gửi.
- Số event consumer đã xử lý.
- Trạng thái producer tasks.
- Trạng thái consumer task sau khi producer kết thúc.

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
[Spoiler — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results
Trước fix, toàn bộ event có thể đã được xử lý nhưng consumer vẫn chờ. Sau fix, `verify.ps1` phải xác nhận đủ 6 event và pipeline hoàn tất trong thời gian giới hạn.

## Estimated Time
45 phút.
