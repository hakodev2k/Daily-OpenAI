# UNIT-TZRULE-001 — Lịch tàu ven biển lệch giờ mùa hè

## Mục tiêu
Điều tra sự khác biệt giữa giờ địa phương trên vé và thời điểm UTC được lưu, phân biệt triệu chứng với giả thuyết, đưa ra sửa đổi có regression test.

## Bối cảnh thực tế
Hệ thống bán vé tàu lưu giờ khởi hành do cảng công bố. Các chuyến mùa đông đúng giờ; một số chuyến mùa hè hiển thị giờ đúng trên vé nhưng bản ghi UTC lệch một giờ. Dịch vụ vẫn trả HTTP 200 và không có exception.

## Bạn cần làm gì
Chạy chương trình, thu thập hai kết quả đối chứng, ghi tối thiểu hai giả thuyết, sửa trong `starter/Program.cs`, rồi chạy verification. Đề xuất quy tắc cho thời điểm địa phương không tồn tại hoặc bị lặp khi đổi đồng hồ.

## Yêu cầu môi trường
.NET SDK 8.0; PowerShell 7 hoặc Windows PowerShell. Không cần database, cloud hoặc mạng.

## Chạy nhanh
```powershell
dotnet run --project starter/FerrySchedule.csproj
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script xác nhận chương trình hiện tại tái hiện sự lệch giữa giờ UTC được lưu và giờ UTC mà quy định cảng yêu cầu. Exit code 0 nghĩa là *reproduce thành công*, không phải ứng dụng đúng.

## Những gì cần quan sát
So sánh giờ địa phương đầu vào, UTC thực tế và UTC kỳ vọng ở hai mùa. Lưu output vào `workspace/my-investigation.md`. Không chỉ dựa vào trạng thái chạy thành công của tiến trình.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** chỉ đọc sau khi tự sửa: [Reference](solution/README.md).

## Expected Results
Starter: kiểm tra mùa đông đúng, mùa hè lệch một giờ. Sau sửa: cả hai mùa khớp UTC kỳ vọng; chính sách xử lý giờ nhập không hợp lệ được nêu rõ.

## Estimated Time
45–60 phút.
