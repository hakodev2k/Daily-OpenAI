# UNIT-ATOMICFILE-001 — Cấu hình tín hiệu đường sắt bị hỏng sau khi ghi gián đoạn

## Mục tiêu
Điều tra tính toàn vẹn của file cấu hình trong một tiến trình .NET, xây dựng bằng chứng và sửa lỗi mà không làm mất bản cấu hình đang hoạt động.

## Bối cảnh thực tế
Một dịch vụ điều phối tín hiệu đường sắt lưu snapshot cấu hình JSON trên ổ đĩa. Khi cập nhật bị gián đoạn, lần khởi động sau đôi khi không đọc được cấu hình. Trong hệ thống thật, sự cố này có thể khiến việc triển khai cấu hình bị dừng; lab chỉ dùng dữ liệu giả lập, không điều khiển thiết bị thật.

## Bạn cần làm gì
1. Chạy kịch bản và ghi lại trạng thái file trước/sau lỗi.
2. Đề xuất ít nhất hai giả thuyết; phân biệt lỗi serialization, quyền file và cách ghi file.
3. Sửa `starter/Program.cs` để bảo toàn snapshot cũ khi cập nhật thất bại.
4. Kiểm tra cập nhật thành công và dọn dẹp file tạm sau lỗi.

## Yêu cầu môi trường
.NET SDK 8.0.x, PowerShell 7 hoặc Windows PowerShell, không cần dịch vụ bên ngoài.

## Chạy nhanh
Tại thư mục unit: `./scripts/run.ps1` (PowerShell); hoặc `dotnet run --project starter/SignalConfigLab.csproj -- verify`.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1`. Script tạo một cấu hình hợp lệ, mô phỏng lỗi giữa quá trình cập nhật và xác nhận triệu chứng hỏng snapshot. Không sử dụng dữ liệu thật.

## Những gì cần quan sát
Ghi lại độ dài file, khả năng parse JSON, trạng thái snapshot cũ, exit code và số file tạm còn sót. Một lần cập nhật thất bại không được khiến bản cấu hình hợp lệ trước đó biến mất.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Chạy verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải tham khảo](solution/README.md)

## Expected Results
Trước sửa: reproduce xác nhận snapshot không còn đọc được; verify thất bại. Sau sửa: verify PASS cho cả lỗi giữa chừng, cập nhật thành công và không để lại file tạm.

## Estimated Time
45–60 phút.
