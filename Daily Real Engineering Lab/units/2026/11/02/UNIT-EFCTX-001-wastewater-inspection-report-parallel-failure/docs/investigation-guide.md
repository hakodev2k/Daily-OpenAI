# Investigation guide — chưa có đáp án

1. Xác minh dữ liệu seed bằng cách đọc `checks/Program.cs`: có ba ticket, hai Open và một Closed.
2. Chạy `./reproduce.ps1`, ghi exception và exit code. Thử suy luận tại sao test dùng gate thay vì `Task.Delay` ngẫu nhiên.
3. Theo dõi `InspectionReportService.BuildAsync`: đếm các operation, nơi chúng được khởi tạo, nơi chúng được await và các resource liên quan.
4. Phân biệt: task chạy song song có đồng nghĩa mọi resource mà task sử dụng đều hỗ trợ concurrent use không?
5. Viết hypothesis, bằng chứng ủng hộ/bác bỏ và dự đoán trước khi sửa.
6. Sau khi sửa, chạy `./verify.ps1` và đối chiếu cả tính đúng đắn lẫn failure path.

Nếu không reproduce: chạy `./setup.ps1`, kiểm tra SDK/NuGet; `dotnet run --project checks/Checks.csproj -- reproduce`; không sửa gate để ép test pass.
