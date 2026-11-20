# UNIT-DSTFOLD-001 — Ca trực tình nguyện bị lệch một giờ

## Mục tiêu
Điều tra hợp đồng giờ địa phương khi múi giờ chuyển mùa; xác định và kiểm chứng một instant trước khi lưu UTC.

## Bối cảnh thực tế
Điều phối viên Toronto đặt ca trực bằng giờ địa phương. Một số phiếu xác nhận sau chuyển giờ không khớp UTC hệ thống lưu.

## Bạn cần làm gì
Reproduce, ghi ít nhất hai giả thuyết, thu thập evidence, sửa `starter/Program.cs`, verify cả lịch bình thường và trường hợp biên.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell. Không cần package, database hay mạng.

## Chạy nhanh
```powershell
dotnet run --project starter/ShiftClock.csproj
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` từ thư mục unit. Script dự kiến exit thành công sau khi phát hiện assertion thất bại trên starter gốc.

## Những gì cần quan sát
Đối chiếu local wall time, các offset, UTC, kết quả xác nhận ca và output từng assertion; không kết luận nguyên nhân trước khi đo.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify với `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md).

## Expected Results
Starter vi phạm ít nhất một điều kiện xác nhận ca. Sau sửa, tất cả sáu assertion PASS, không có regression ở ca bình thường.

## Estimated Time
50 phút.
