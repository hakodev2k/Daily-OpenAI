# Reference Solution — mở sau khi hoàn thành

## Root cause
`HttpClient.DefaultRequestHeaders` là cấu hình mặc định của client dùng chung, không phải nơi lưu ngữ cảnh tenant của từng request. `Prepare` của tenant B ghi đè trạng thái client trước khi cả hai delegate gửi request. Kết quả hai request cùng nhận header tenant B. Việc thay đổi default headers khi có requests đang hoạt động còn không an toàn về concurrency.

## Fix
Trong `solution/Program.cs`, mỗi lần gửi tạo một `HttpRequestMessage` riêng, đặt `X-Tenant-Id` vào `request.Headers`, rồi gọi `SendAsync`. Giữ lại HttpClient pooling, không tạo client mới theo từng invoice.

## Verify
Chạy `./verify.ps1` sau khi sửa `starter/Program.cs`; `dotnet run --project solution/Lab.csproj` là reference độc lập. Cả hai phải trả đúng `invoice-100:tenant-alpha` và `invoice-200:tenant-beta` và exit code 0.

## Wrong fixes
- Thêm `lock` quanh `DefaultRequestHeaders.Add` nhưng nhả khóa trước `SendAsync`: trạng thái vẫn có thể bị thay đổi.
- Tạo `new HttpClient()` cho từng yêu cầu: che triệu chứng nhưng làm xấu quản lý kết nối và DNS.
- Chỉ thêm retry: lỗi phân quyền/dữ liệu không phải lỗi transient.

## Trade-offs và production guardrails
Trong hệ thống thực, tenant identity phải được xác thực từ principal/token phía server; không tin header do caller tùy ý truyền. Kiểm thử cạnh tranh nhiều request với fake handler; không log secret/token; dùng request-specific correlation ID, và test âm tính bảo đảm tenant khác không đọc được hóa đơn. Nếu handler có quyền truy cập tài nguyên thực, bắt buộc xác thực authorization ở downstream, không chỉ dựa vào header.
