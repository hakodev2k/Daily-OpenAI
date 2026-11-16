# UNIT-STREAMREAD-001 — Header telemetry từ trạm thời tiết bị giải mã sai

## Mục tiêu
Điều tra lỗi đọc khung nhị phân từ nguồn I/O phân mảnh; thu thập evidence và sửa đường code đang chạy.

## Bối cảnh thực tế
Gateway tiếp nhận header 4 byte little-endian chứa sequence number. Một số frame bị đánh dấu là packet cũ dù phía gửi không mất byte.

## Bạn cần làm gì
Reproduce, ghi bytes-read, decoded-sequence và expected-sequence; đưa ra hai giả thuyết, sửa `starter/Program.cs` và verify.

## Yêu cầu môi trường
.NET SDK 8.0 và PowerShell; không cần NuGet bên ngoài, Docker hoặc cloud.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` thành công khi tái hiện mismatch gốc; **không** có nghĩa đã fix.

## Những gì cần quan sát
Đối chiếu số byte đọc được với số byte header kỳ vọng. Kiểm tra byte ở nguồn và byte sau giải mã, phân biệt mất dữ liệu và sai cách diễn giải.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Xem gợi ý theo mức độ](hints.md).

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md).

## Expected Results
Trước sửa: reproduce thành công và verify thất bại. Sau sửa: verify thành công và reproduce không còn phát hiện lỗi gốc.

## Estimated Time
40 phút.
