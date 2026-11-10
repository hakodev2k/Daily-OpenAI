# UNIT-ROUNDLINE-001 — Sai lệch đối soát hóa đơn tín chỉ điện mặt trời

## Mục tiêu
Reproduce chênh lệch 0,01–0,02 VND ở hệ thống tính tiền theo dòng; phân biệt tính đúng theo hợp đồng với kết quả số học trông có vẻ hợp lý.

## Bối cảnh thực tế
Hợp tác xã điện mặt trời phân bổ tín chỉ cho các hộ dân. Bản xem trước hóa đơn và sổ đối soát độc lập cho ra tổng khác nhau ở một số hóa đơn có nhiều dòng nhỏ, dù dữ liệu đầu vào giống nhau.

## Bạn cần làm gì
Chạy bộ mẫu, ghi kết quả từng dòng và tổng, đưa ra ít nhất hai giả thuyết, sửa đường tính trong `starter/` để thỏa hợp đồng trong `contract.md` mà không làm sai trường hợp số âm.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell 7+ (không có NuGet ngoài).

## Chạy nhanh
```powershell
dotnet run --project starter/SolarCredits.csproj
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trong thư mục unit; script chạy chương trình gốc và xác nhận có sai lệch so với dữ liệu đối soát.

## Những gì cần quan sát
Đối chiếu số tiền từng dòng, số tiền của hóa đơn, dấu của credit và độ lệch khi thay đổi thứ tự các dòng. Lưu output trước khi chỉnh code.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải tham khảo](solution/README.md) — chỉ xem sau khi tự thử.

## Expected Results
Starter: có ít nhất một hóa đơn lệch đối soát. Sau sửa: cả ba tình huống trong contract khớp chính xác.

## Estimated Time
35–45 phút.
