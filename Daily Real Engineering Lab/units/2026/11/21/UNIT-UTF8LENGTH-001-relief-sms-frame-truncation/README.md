# UNIT-UTF8LENGTH-001 — Gói tin cứu trợ bị cắt khi gửi tiếng Việt

## Mục tiêu
Điều tra lỗi framing ở ranh giới giữa ứng dụng .NET và một gateway nhận dữ liệu nhị phân; thu thập bằng chứng ở mức byte và bảo vệ tính toàn vẹn của hai bản tin liên tiếp.

## Bối cảnh thực tế
Dịch vụ điều phối cứu trợ gửi nội dung SMS theo giao thức nội bộ: 4 byte độ dài (little-endian) và payload UTF-8. Bản tin ASCII đi qua, nhưng một số thông báo tiếng Việt bị cắt; bản tin kế tiếp có thể không đọc được.

## Bạn cần làm gì
1. Chạy bản gốc, ghi lại kết quả của hai bản tin.
2. Đưa ra ít nhất hai giả thuyết, so sánh header và payload thực tế.
3. Chỉnh sửa `starter/Program.cs` để cả hai bản tin đọc đúng, không thay đổi wire protocol.
4. Chạy verify, giải thích bất biến cần giữ ở biên protocol.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell 5.1; không cần database, cloud hay Internet sau khi có SDK.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` từ thư mục unit; script kiểm tra hiện tượng thất bại dự kiến trên starter gốc.

## Những gì cần quan sát
So sánh declared length, số byte thực tế, số frame đọc được, và nội dung Unicode sau round-trip. Không dựa vào số ký tự hiển thị.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Đọc sau khi tự làm](solution/README.md).

## Expected Results
Trước: ít nhất một bản tin lỗi hoặc stream không thể giải mã đầy đủ. Sau: hai payload round-trip chính xác, không còn byte thừa, hỗ trợ ASCII và tiếng Việt.

## Estimated Time
40–50 phút.
