# UNIT-ARRAYLEASE-001 — Bản tin vô tuyến bị thay đổi sau khi xếp hàng gửi

## Mục tiêu
Điều tra lỗi toàn vẹn payload tại ranh giới producer–consumer; thu thập bằng chứng, sửa code và đánh giá chi phí cấp phát bộ nhớ.

## Bối cảnh thực tế
Trạm cứu hộ vùng núi gom bản tin ngắn trước khi chuyển tới bộ phát vô tuyến. Log tiếp nhận ghi `ALERT`, nhưng một bản tin được phát lại có nội dung khác. Không có mạng thật trong lab.

## Bạn cần làm gì
Chạy hệ thống, lưu lại kết quả, đưa ra ít nhất hai hypothesis, xác định điều kiện vòng đời dữ liệu, sửa phần `starter/` và chứng minh cả bản tin gốc lẫn bản tin kế tiếp vẫn đúng.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell. Không cần Docker, cloud hay NuGet package bên ngoài.

## Chạy nhanh
Tại thư mục unit:
```powershell
dotnet build starter/RadioDispatch.csproj
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script phải in `REPRODUCED` và trả exit code 0 khi phát hiện payload khác nội dung đã tiếp nhận.

## Những gì cần quan sát
So sánh `accepted` và `transmitted` trong console; xem thứ tự sự kiện `queued`, `reused`, `drained`. Đánh dấu chính xác thời điểm dữ liệu không còn ổn định.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1` để kiểm chứng đường code bạn sửa.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
[Cảnh báo spoiler — lời giải và trade-offs](solution/README.md)

## Expected Results
[Trước sửa](expected-results/before.md) · [Sau sửa](expected-results/after.md). `verify.ps1` chỉ PASS khi cả hai payload chính xác.

## Estimated Time
45 phút.
