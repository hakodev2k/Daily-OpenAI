# UNIT-TASKCACHE-001 — Configuration cache không phục hồi sau sự cố tạm thời

## Mục tiêu
Điều tra vòng đời kết quả bất đồng bộ, phân biệt lỗi nhất thời với trạng thái cache lâu dài và kiểm chứng khả năng phục hồi của một dịch vụ .NET.

## Bối cảnh thực tế
Dịch vụ cấp học bổng của một quận lấy cấu hình chính sách từ endpoint nội bộ. Lần gọi đầu sau khởi động gặp sự cố nhất thời. Endpoint sau đó đã phục hồi, nhưng tiến trình vẫn không đọc được chính sách.

## Bạn cần làm gì
Chạy hai lượt truy cập trên cùng một instance, ghi lại output, đưa ra ít nhất hai giả thuyết, sửa mã trong `starter/Program.cs` và kiểm chứng hành vi. Giữ khả năng dùng chung kết quả thành công cho các lượt gọi sau.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell; không cần Docker, tài khoản Azure hay API trả phí.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Từ thư mục unit, chạy `./scripts/reproduce.ps1`. Script chạy **starter chưa sửa** và yêu cầu cả hai lượt đều thất bại; nếu không, báo lỗi vì không reproduce được. Không dùng script này để chứng minh bản sửa.

## Những gì cần quan sát
Ghi lại kết quả của `initial` và `recovery`, số lần backend được gọi nếu cần bổ sung instrumentation, và trạng thái sau khi một request lỗi. Quan sát việc lỗi còn tồn tại dù nguồn dữ liệu có thể đáp ứng ở lần sau.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ xem [solution/README.md](solution/README.md) sau khi tự điều tra.

## Expected Results
Trước sửa: hai lượt cùng báo lỗi. Sau sửa: lượt đầu báo lỗi tạm thời, lượt tiếp theo nhận `grant-policy:v2`; lượt thành công sau đó có thể được tái sử dụng.

## Estimated Time
35–45 phút.
