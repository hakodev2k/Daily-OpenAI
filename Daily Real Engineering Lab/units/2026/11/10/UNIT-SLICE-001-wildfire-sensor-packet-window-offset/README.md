# UNIT-SLICE-001 — Wildfire sensor: sai cửa sổ payload trong bộ đệm tái sử dụng

## Mục tiêu
Tự reproduce lỗi giải mã dữ liệu nhị phân từ một vùng con của receive buffer, thu thập bằng chứng, sửa mã và kiểm tra boundary contract.

## Bối cảnh thực tế
Gateway nhận các gói đo nhiệt độ từ cảm biến giám sát cháy rừng. Một số thiết bị gửi payload đúng nhưng dashboard hiển thị số đo không tương ứng với payload trên wire. Hệ thống tái sử dụng buffer lớn để giảm allocation.

## Bạn cần làm gì
Chạy fixture, ghi lại số đo thực tế và số đo kỳ vọng, đưa ra ít nhất hai giả thuyết; chỉ sửa `starter/Program.cs`, sau đó verify.

## Yêu cầu môi trường
.NET SDK 8.0.x, PowerShell 7 hoặc Windows PowerShell; không cần database hay Azure.

## Chạy nhanh
```powershell
dotnet run --project starter/SensorGateway.csproj -- --demo
./reproduce.ps1
```

## Cách reproduce vấn đề
Từ thư mục unit chạy `./reproduce.ps1`. Script cần thấy ít nhất một fixture lỗi; mã thoát 0 nghĩa là đã tái hiện được vấn đề.

## Những gì cần quan sát
So sánh frame ID, vị trí payload trong buffer, expected và actual; kiểm tra liệu dữ liệu có thay đổi khi buffer có prefix khác nhau hay không. Không dựa vào timing.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix trên `starter/`.
4. Chạy `./verify.ps1` để kiểm tra chính mã đã sửa.
5. Chỉ sau đó xem `solution/README.md`.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md).

## Expected Results
Trước: ít nhất một fixture sai. Sau: tất cả fixture hợp lệ trả đúng nhiệt độ và fixture không đủ byte bị từ chối.

## Estimated Time
35 phút.
