# UNIT-CULTUREKEY-001 — Lệnh điều độ đường sắt thay đổi kết quả theo culture

## Mục tiêu
Điều tra một lỗi tính nhất quán trong bộ định tuyến lệnh C# khi cùng một payload đi qua hai máy chủ cấu hình khác nhau.

## Bối cảnh thực tế
Hệ thống điều độ đường sắt nhận mã lệnh từ thiết bị hiện trường. Trong staging một lệnh được chấp nhận, nhưng ở một vùng triển khai khác bị từ chối dù payload không đổi.

## Bạn cần làm gì
Chạy mẫu, ghi lại đầu ra của hai môi trường, đưa ra ít nhất hai giả thuyết, sửa `starter/Program.cs` và kiểm tra cả hành vi thành công lẫn lệnh không hợp lệ.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell 7 hoặc Windows PowerShell 5.1. Không cần database, cloud hay dịch vụ trả phí.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Từ thư mục unit, chạy `./reproduce.ps1`. Script kiểm tra triệu chứng ở starter nguyên bản và báo lỗi nếu triệu chứng không xuất hiện.

## Những gì cần quan sát
So sánh `culture`, `command` và `route` ở hai dòng log. Ghi nhận dữ liệu đầu vào có thay đổi không. Đối chiếu với hợp đồng lệnh không phụ thuộc vị trí máy chủ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và bằng chứng.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải tham khảo](solution/README.md)

## Expected Results
Trước sửa: cùng lệnh có thể được định tuyến khác nhau. Sau sửa: lệnh hợp lệ có cùng kết quả trên cả hai môi trường; lệnh lạ vẫn bị từ chối.

## Estimated Time
35 phút.
