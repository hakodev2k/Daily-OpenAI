# UNIT-PIPESEG-001 — Trạm thời tiết nhận thiếu bản ghi khi dữ liệu đi qua nhiều buffer

## Mục tiêu
Điều tra tính toàn vẹn bản ghi khi xử lý payload từ tầng I/O; chứng minh bằng nhiều cách phân mảnh dữ liệu thay vì chỉ kiểm tra một happy path.

## Bối cảnh thực tế
Gateway nhận batch số đo từ ba trạm thời tiết. Môi trường test thường nhận đủ ba bản ghi; sau khi đổi transport, một số batch chỉ ghi nhận một trạm dù tổng byte gửi đi không đổi. Nhóm vận hành cần giải thích sự chênh lệch và giữ nguyên giao thức `StationId|Celsius\n`.

## Bạn cần làm gì
Chạy starter, ghi nhận số bản ghi trong các cấu hình buffer khác nhau, đặt ít nhất hai giả thuyết, sửa `starter/Program.cs` rồi kiểm tra hồi quy. Không thay đổi fixture hoặc giảm số lượng assertion.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell, không cần database, Azure hay NuGet bên ngoài.

## Chạy nhanh
Tại thư mục unit:
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```
Sau khi sửa:
```powershell
./scripts/verify.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy nguyên trạng starter với cùng nội dung payload được chia thành nhiều phần. Exit code 0 nghĩa là triệu chứng gốc đã được tái hiện; không có nghĩa hệ thống đã đúng.

## Những gì cần quan sát
So sánh số byte nguồn, số bản ghi giải mã và sự khác nhau giữa một buffer với nhiều buffer. Ghi lại điều kiện nào làm sai lệch kết quả. Không suy luận lỗi từ một log duy nhất.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ đọc sau khi tự sửa và verify: [Reference Solution](solution/README.md).

## Expected Results
Trước sửa: một số cách phân mảnh cùng payload cho số bản ghi không đủ. Sau sửa: tất cả ba cấu hình nhận đúng S01=24, S02=27, S03=29; `verify.ps1` trả 0.

## Estimated Time
40–50 phút.
