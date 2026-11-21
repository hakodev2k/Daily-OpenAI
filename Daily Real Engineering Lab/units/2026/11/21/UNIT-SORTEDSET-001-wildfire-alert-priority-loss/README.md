# UNIT-SORTEDSET-001 — Cảnh báo cháy rừng biến mất khỏi hàng đợi ưu tiên

## Mục tiêu
Tìm nguyên nhân một số cảnh báo bị thiếu dù input hợp lệ, xây dựng bằng chứng và regression test.

## Bối cảnh thực tế
Trung tâm điều phối nhận cảnh báo từ nhiều trạm quan trắc cháy rừng. Trong đợt cao điểm, dashboard nhận 5 cảnh báo nhưng chỉ hiển thị một phần; không có exception.

## Bạn cần làm gì
Chạy replay, so sánh ID đầu vào/đầu ra, ghi ít nhất hai giả thuyết, sửa `starter/Program.cs` và verify. Giữ thứ tự Severity giảm dần, ID tăng dần; duplicate ID chỉ xử lý một lần.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell; không cần mạng hoặc dịch vụ trả phí.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Từ thư mục unit, chạy `./reproduce.ps1`. Exit code 0 chỉ xác nhận đã tái hiện lỗi, không xác nhận đã sửa.

## Những gì cần quan sát
Đối chiếu số lượng và ID, thử đảo thứ tự input; ghi nhận kết quả trước khi đọc hints.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[01](hints/hint-01.md) · [02](hints/hint-02.md) · [03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md) — chỉ xem sau khi thử sửa.

## Expected Results
Trước sửa: số ID được dispatch nhỏ hơn số ID đầu vào. Sau sửa: đủ 5 ID theo thứ tự ưu tiên; `./verify.ps1` in `VERIFY_PASS` và trả exit code 0.

## Estimated Time
40 phút.
