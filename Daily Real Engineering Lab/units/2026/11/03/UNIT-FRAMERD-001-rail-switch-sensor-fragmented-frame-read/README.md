# UNIT-FRAMERD-001 — Khung telemetry hợp lệ bị từ chối khi đường truyền chia nhỏ dữ liệu

## Mục tiêu
Refactor bộ đọc frame nhị phân trong .NET 8, dựa trên bằng chứng tái hiện lỗi; bảo vệ tính toàn vẹn dữ liệu, ranh giới độ dài, EOF và cancellation mà không thay đổi wire protocol.

## Bối cảnh thực tế
Một gateway nhận trạng thái công tắc đường ray từ bộ điều khiển cũ. Payload UTF-8 được đóng gói bằng 4 byte độ dài big-endian, theo sau bởi đúng số byte dữ liệu. Trong staging dùng dữ liệu lưu sẵn, kết quả ổn định; khi chuyển sang nguồn mạng có các mảnh dữ liệu nhỏ, gateway đôi khi báo frame thiếu dữ liệu dù bên gửi khẳng định đã gửi đủ.

## Bạn cần làm gì
1. Chạy baseline và ghi nhận hai kết quả trái ngược giữa nguồn liên tục và nguồn phân mảnh.
2. Đọc implementation trong `starter/FrameDecoder.cs`, thu thập chứng cứ từ harness và lập ít nhất hai hypothesis.
3. Sửa **chỉ** `starter/FrameDecoder.cs`, không sửa wire protocol hoặc harness.
4. Chạy verification trên chính source đã sửa, kiểm tra normal/fragmented/truncated/invalid-length/cancellation.
5. Sau khi tự hoàn thành, mở lời giải tham khảo để so sánh trade-off.

## Yêu cầu môi trường
Windows PowerShell 7 hoặc Windows PowerShell 5.1; .NET SDK 8 trở lên có khả năng build `net8.0`. Không cần Docker, database, tài khoản cloud hoặc NuGet bên ngoài.

## Chạy nhanh
Từ thư mục unit:
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
./verify.ps1
```
`reproduce.ps1` chạy **baseline bất biến** trong harness. `verify.ps1` chạy code học viên trong `starter/`, sẽ FAIL trước khi sửa.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Baseline đọc được cùng frame từ nguồn liên tục, nhưng từ nguồn có cùng byte sequence được chia thành nhiều mảnh lại báo lỗi. Kết quả `REPRODUCED` và exit code 0 xác nhận lỗi baseline đã xuất hiện; không có nghĩa code học viên đã pass.

## Những gì cần quan sát
- So sánh byte length thực tế của frame với thông báo lỗi.
- So sánh hành vi khi cùng một frame được cấp qua hai kiểu Stream.
- Quan sát case nào thất bại trong `./verify.ps1` trước và sau khi sửa.
- Ghi lại sự khác biệt giữa dữ liệu chưa đến đủ và dữ liệu thực sự kết thúc sớm.

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
**Spoiler:** chỉ xem sau khi đã thử sửa: [Reference Solution](solution/README.md).

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
65 phút.
