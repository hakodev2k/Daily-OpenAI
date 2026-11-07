# UNIT-AZURE-007 — Rotated Secret Is Not Observed by a Long-Running Worker

## Mục tiêu
Điều tra vì sao một worker chạy lâu vẫn hoạt động bình thường lúc khởi động nhưng bắt đầu bị downstream từ chối sau khi credential được rotate.

## Bối cảnh thực tế
Một settlement worker gọi partner API theo chu kỳ. Operations có thể thay đổi credential trong lúc process vẫn chạy. Sau một lần rotation, dashboard cho thấy partner đã nhận credential mới nhưng worker hiện tại vẫn liên tục nhận 401 cho đến khi service được restart.

## Bạn cần làm gì
Reproduce hiện tượng, ghi hypothesis, xác định lifecycle của dữ liệu cấu hình trong process, sửa starter để lần xử lý tiếp theo có thể quan sát credential mới mà không cần restart.

## Yêu cầu môi trường
- .NET SDK 8.0.x
- PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy script reproduce. Lab dùng local deterministic credential provider, không cần Azure subscription. Script chạy hai settlement cycle và rotate credential giữa hai cycle.

## Những gì cần quan sát
- Cycle đầu thành công.
- Credential source thay đổi trước cycle tiếp theo.
- Cycle sau vẫn có thể dùng giá trị khác với giá trị hiện tại của source.
- Quan sát thời điểm application đọc configuration và thời điểm dependency sử dụng nó.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results
Trước fix, cycle sau rotation bị từ chối. Sau fix, `verify.ps1` báo `VERIFY_PASS` và worker quan sát credential mới ở processing boundary phù hợp.

## Estimated Time
45 phút.
